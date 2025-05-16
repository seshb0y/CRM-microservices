import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axiosInstance from "../../api/axiosInstance";
import { User, UserRole } from "../../types/User";
import { toast } from 'react-toastify';

interface UsersState {
  users: User[];
  loading: boolean;
  error: string | null;
  userLoading: boolean;
  userError: string | null;
  userCreating: boolean;
  userCreateError: string | null;
}

const initialState: UsersState = {
  users: [],
  loading: true,
  error: null,
  userLoading: false,
  userError: null,
  userCreating: false,
  userCreateError: null,
};

export const deleteUser = createAsyncThunk(
  "users/delete",
  async (email: string, { rejectWithValue }) => {
    try {
      await axiosInstance.delete(`/users/`, {data: {email: email} });
      toast.success('Пользователь успешно удален');
      return email;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (error: any) {
      toast.error(error.response?.data || "Ошибка при удалении пользователя");
      return rejectWithValue(error.response?.data || "Failed to delete user");
    }
  }
);

export const fetchChangeUserData = createAsyncThunk(
  "users/data/change",
  async ({ username, newEmail, oldEmail, role }: { 
    username: string; 
    newEmail: string; 
    oldEmail: string; 
    role: typeof UserRole[keyof typeof UserRole] 
  }, { rejectWithValue }) => {
    try {
      const response = await axiosInstance.put(`/users/`, { 
        username, 
        newEmail, 
        oldEmail, 
        role
      });
      toast.success('Данные пользователя успешно обновлены');
      return response.data; 
    } catch (error: unknown) {
      const err = error as { response?: { data?: unknown } };
      const errorMessage = typeof err.response?.data === 'string' 
        ? err.response.data 
        : "Ошибка при обновлении данных пользователя";
      toast.error(errorMessage);
      return rejectWithValue(errorMessage);
    }
  }
);

export const fetchUsers = createAsyncThunk(
  "users/fetch",
  async ({ sortBy, descending }: { sortBy?: string; descending?: boolean }) => {
    const params = new URLSearchParams();
    if (sortBy) params.append("sortBy", sortBy);
    if (descending !== undefined) params.append("Descending", descending.toString());
    const response = await axiosInstance.get(`/users?${params.toString()}`);
    return response.data;
  }
);

export const fetchAddUserData = createAsyncThunk(
  "user/load/client/data",
  async ({ email }: { email: string }, { rejectWithValue }) => {
    try {
      const response = await axiosInstance.get(`users/search?email=${encodeURIComponent(email)}`);
      return response.data;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (error: any) {
      return rejectWithValue(error.response?.data || "Failed to load user data");
    }
  }
);

export const createUser = createAsyncThunk(
  "users/create",
  async (userData: { username: string; password: string; email: string; confirmPassword: string }, { rejectWithValue }) => {
    try {
      console.log(userData)
      const response = await axiosInstance.post("/account/register", userData);
      toast.success('Пользователь успешно создан');
      return response.data;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (error: any) {
      toast.error(error.response?.data || "Ошибка при создании пользователя");
      return rejectWithValue(error.response?.data || "Failed to create user");
    }
  }
);

// export const fetchClientsWithOrdersAndTasks = createAsyncThunk(
//   "clients/fetchClientsWithOrdersAndTasks",
//   async (_, { rejectWithValue }) => {
//     try {
//       const response = await axiosInstance.get("/Client/Get/Clients/With/Orders/And/Tasks");
//       console.log(response.data)
//       return response.data;
//     // eslint-disable-next-line @typescript-eslint/no-explicit-any
//     } catch (error: any) {
//       return rejectWithValue(error.response?.data || "Failed to fetch clients with orders and tasks");
//     }
//   }
// );

const usersSlice = createSlice({
  name: "users",
  initialState,
  reducers: {
    addUserRealtime: (state, action) => {
      const exists = state.users.find(user => 
        user.userId === action.payload.userId || 
        user.email === action.payload.email
      );
      if (!exists) {
        const newUser = {
          ...action.payload,
          userId: action.payload.id || action.payload.userId, // Поддерживаем оба варианта id
          userRole: action.payload.role || action.payload.userRole, // Поддерживаем оба варианта role
          isEmailConfirmed: action.payload.isEmailConfirmed || false
        };
        state.users.push(newUser);
      }
    },
    changeUserRealtime: (state, action) => {
      const index = state.users.findIndex(user => user.userId === action.payload.userId);
      if (index !== -1) {
        state.users[index] = action.payload;
      }
    },
    deleteUserRealtime: (state, action) => {
      state.users = state.users.filter(user => user.userId !== action.payload.userId);
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchUsers.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchUsers.fulfilled, (state, action) => {
        state.users = Array.isArray(action.payload.users) ? action.payload.users : [];
        state.loading = false;
      })
      .addCase(fetchUsers.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })

    //   .addCase(fetchClientsWithOrdersAndTasks.pending, (state) => {
    //     state.loading = true;
    //     state.error = null;
    //   })
    //   .addCase(fetchClientsWithOrdersAndTasks.fulfilled, (state, action) => {
    //     state.users = action.payload; 
    //     state.loading = false;
    //   })
    //   .addCase(fetchClientsWithOrdersAndTasks.rejected, (state, action) => {
    //     state.loading = false;
    //     state.error = action.payload as string;
    //   })

      .addCase(fetchAddUserData.pending, (state) => {
        state.userLoading = true;
        state.userError = null;
      })
      .addCase(fetchAddUserData.fulfilled, (state, action) => {
        const userIndex = state.users.findIndex(user => user.email === action.meta.arg.email);
        if (userIndex !== -1) {
          state.users[userIndex] = {
            ...state.users[userIndex],
            orders: action.payload.orders || [],
            tasks: action.payload.tasks || []
          };
        }
        state.userLoading = false;
      })
      .addCase(fetchAddUserData.rejected, (state, action) => {
        state.userLoading = false;
        state.userError = action.payload as string;
      })

      .addCase(fetchChangeUserData.pending, (state) => {
        state.userLoading = true;
        state.userError = null;
      })
      .addCase(fetchChangeUserData.fulfilled, (state, action) => {
        const index = state.users.findIndex(user => user.email === action.meta.arg.oldEmail);
        if (index !== -1) {
          const updatedUser = {
            ...state.users[index],
            username: action.payload.username,
            email: action.payload.email,
            userRole: action.payload.role,
            isEmailConfirmed: action.payload.isEmailConfirmed
          };
          state.users[index] = updatedUser;
        }
        state.userLoading = false;
      })
      .addCase(fetchChangeUserData.rejected, (state, action) => {
        state.userLoading = false;
        state.userError = action.payload as string;
      })

      .addCase(deleteUser.fulfilled, (state, action) => {
        state.users = state.users.filter(user => user.email !== action.payload);
      })
      .addCase(deleteUser.rejected, (state, action) => {
        state.error = action.payload as string;
      })

      .addCase(createUser.fulfilled, (state) => {
        state.userCreating = false;
        state.userCreateError = null;
      })
      .addCase(createUser.rejected, (state, action) => {
        state.userCreateError = action.payload as string;
        state.userCreating = false;
      })
  },
});

export const { addUserRealtime, changeUserRealtime, deleteUserRealtime } = usersSlice.actions;
export default usersSlice.reducer;

export const selectUserById = (state: { users: UsersState }, id: number) => 
  state.users.users.find(user => user.userId === String(id));
