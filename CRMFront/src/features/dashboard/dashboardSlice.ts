import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
import axiosInstance from '../../api/axiosInstance';
import { TaskStatus } from '../../types/Task';


interface DashboardState {
  clientsAmount: number;
  ordersCount: number;
  ordersTotalAmount: number;
  tasksCount: number;
  tasksStatuses: TaskStatus[],
  loading: boolean;
  ordersCreatedDates: string[];
  isError: boolean;
}

export const getDashboardData = createAsyncThunk("dashboardState", async () => {
  const response = await axiosInstance.get("/clients/dashboard");
  return response.data
})

const initialState: DashboardState = {
  clientsAmount: 0,
  ordersCount: 0,
  tasksCount: 0,
  ordersTotalAmount: 0,
  loading: false,
  ordersCreatedDates: [],
  tasksStatuses: [],
  isError: false,
};

const dashboardSlice = createSlice({
  name: 'dashboard',
  initialState,
  reducers: {
    setDashboardData: (state, action: PayloadAction<Partial<DashboardState>>) => {
      return { ...state, ...action.payload };
    },
  },
  extraReducers: (builder) => {
    builder
    .addCase(getDashboardData.pending, (state) => {
      state.loading = true;
    })
    .addCase(getDashboardData.fulfilled, (state, action) => {
      state.loading = false;
      state.clientsAmount = action.payload.clientsAmount;
      state.ordersCount = action.payload.ordersCount;
      state.ordersTotalAmount = action.payload.ordersTotalAmount;
      state.tasksCount = action.payload.tasksCount;
      state.ordersCreatedDates = action.payload.ordersCreatedDates;
      state.tasksStatuses = action.payload.tasksStatuses;
    })
    
    .addCase(getDashboardData.rejected, (state) => {
      state.loading = false;
      state.isError = true;
    })
  }
});

export const { setDashboardData } = dashboardSlice.actions;
export default dashboardSlice.reducer;
