import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axiosInstance from "../../api/axiosInstance";
import { Client } from "../../types/Client";
import { toast } from 'react-toastify';

interface ClientsState {
  clients: Client[];
  loading: boolean;
  error: string | null;
  clientLoading: boolean;
  clientError: string | null;
  clientCreating: boolean;
  clientCreateError: string | null;
}

const initialState: ClientsState = {
  clients: [],
  loading: true,
  error: null,
  clientLoading: false,
  clientError: null,
  clientCreating: false,
  clientCreateError: null,
};

export const deleteClient = createAsyncThunk(
  "clients/delete",
  async (email: string, { rejectWithValue }) => {
    try {
      await axiosInstance.delete(`/clients/`, {data: {email: email} });
      toast.success('Клиент успешно удален');
      return email;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (error: any) {
      console.error('Error deleting client:', error);
      toast.error(error.response?.data || "Ошибка при удалении клиента");
      return rejectWithValue(error.response?.data || "Failed to delete client");
    }
  }
);

export const fetchChangeClientData = createAsyncThunk(
  "client/data/change",
  async ({ 
    name,
    newEmail,
    oldEmail,
    phone,
    address 
  }: { 
    name: string;
    newEmail: string;
    oldEmail: string;
    phone: string;
    address: string;
  }, { rejectWithValue }) => {
    try {
      const response = await axiosInstance.put(`/clients/`, { 
        name,
        newEmail,
        oldEmail,
        phone,
        address
      });
      toast.success('Данные клиента успешно обновлены');
      return response.data;
    } catch (error: unknown) {
      const err = error as { response?: { data?: unknown } };
      const errorMessage = typeof err.response?.data === 'string' 
        ? err.response.data 
        : "Ошибка при обновлении данных клиента";
      toast.error(errorMessage);
      return rejectWithValue(errorMessage);
    }
  }
);

export const fetchClients = createAsyncThunk(
  "clients/fetchClients",
  async ({ sortBy, descending }: { sortBy?: string; descending?: boolean }) => {
    const params = new URLSearchParams();
    if (sortBy) params.append("sortBy", sortBy);
    if (descending !== undefined) params.append("Descending", descending.toString());

    const response = await axiosInstance.get(`/clients/?${params.toString()}`);
    return response.data;
  }
);

export const fetchAddClientData = createAsyncThunk(
  "clients/load/client/data",
  async ({ email }: { email: string }, { rejectWithValue }) => {
    try {
      const response = await axiosInstance.get(`clients/search?email=${encodeURIComponent(email)}`);
      return response.data;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (error: any) {
      return rejectWithValue(error.response?.data || "Failed to load client data");
    }
  }
);

export const createClient = createAsyncThunk(
  "clients/create",
  async (clientData: { 
    name: string;
    email: string;
    phone: string;
    address: string;
    createdAt: string;
  }, { rejectWithValue }) => {
    try {
      const response = await axiosInstance.post("/clients/", clientData);
      toast.success('Клиент успешно создан');
      return response.data;
    } catch (error: unknown) {
      const err = error as { response?: { data?: unknown } };
      toast.error((err.response?.data as string) || "Ошибка при создании клиента");
      return rejectWithValue(err.response?.data || "Failed to create client");
    }
  }
);

export const fetchClientsWithOrdersAndTasks = createAsyncThunk(
  "clients/fetchClientsWithOrdersAndTasks",
  async (_, { rejectWithValue }) => {
    try {
      const response = await axiosInstance.get("/clients/relations");
      return response.data;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (error: any) {
      return rejectWithValue(error.response?.data || "Failed to fetch clients with orders and tasks");
    }
  }
);

const clientsSlice = createSlice({
  name: "clients",
  initialState,
  reducers: {
    addClientRealtime: (state, action) => {
      const exists = state.clients.find(client => client.id === action.payload.id);
      if (!exists) {
        state.clients.push(action.payload);
      }
    },
    changeClientRealtime: (state, action) => {
      const index = state.clients.findIndex(client => client.id === action.payload.id);
      if (index !== -1) {
        state.clients[index] = action.payload;
      }
    },
    deleteClientRealtime: (state, action) => {
      state.clients = state.clients.filter(client => client.id !== action.payload.id);
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchClients.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchClients.fulfilled, (state, action) => {
        state.clients = Array.isArray(action.payload.clients) ? action.payload.clients : [];
        state.loading = false;
      })
      .addCase(fetchClients.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })

      .addCase(fetchClientsWithOrdersAndTasks.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchClientsWithOrdersAndTasks.fulfilled, (state, action) => {
        state.clients = action.payload; 
        state.loading = false;
      })
      .addCase(fetchClientsWithOrdersAndTasks.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })

      .addCase(fetchAddClientData.pending, (state) => {
        state.clientLoading = true;
        state.clientError = null;
      })
      .addCase(fetchAddClientData.fulfilled, (state, action) => {
        const clientIndex = state.clients.findIndex(client => client.email === action.meta.arg.email);
        if (clientIndex !== -1) {
          state.clients[clientIndex] = {
            ...state.clients[clientIndex],
            orders: action.payload.orders || [],
            users: action.payload.users || []
          };
        }
        state.clientLoading = false;
      })
      .addCase(fetchAddClientData.rejected, (state, action) => {
        state.clientLoading = false;
        state.clientError = action.payload as string;
      })

      .addCase(fetchChangeClientData.pending, (state) => {
        state.clientLoading = true;
        state.clientError = null;
      })
      .addCase(fetchChangeClientData.fulfilled, (state, action) => {
        const index = state.clients.findIndex(client => client.id === action.payload.id);
        if (index !== -1) {
          state.clients[index] = action.payload; 
        }
        state.clientLoading = false;
      })
      .addCase(fetchChangeClientData.rejected, (state, action) => {
        state.clientLoading = false;
        state.clientError = action.payload as string;
      })

      .addCase(deleteClient.fulfilled, (state, action) => {
        state.clients = state.clients.filter(client => client.email !== action.payload);
      })
      .addCase(deleteClient.rejected, (state, action) => {
        state.error = action.payload as string;
      });
  },
});

export const { addClientRealtime, changeClientRealtime, deleteClientRealtime } = clientsSlice.actions;
export default clientsSlice.reducer;

