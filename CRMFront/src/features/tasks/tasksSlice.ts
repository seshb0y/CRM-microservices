import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axiosInstance from "../../api/axiosInstance";
import { CreateTask, Task, TaskStatus } from "../../types/Task";

interface TaskState {
  tasks: Task[];
  loading: boolean;
  error: string | null;
  taskLoading: boolean;
  taskError: string | null;
  taskCreating: boolean;
  taskCreateError: string | null;
}

const initialState: TaskState = {
  tasks: [],
  loading: true,
  error: null,
  taskLoading: false,
  taskError: null,
  taskCreating: false,
  taskCreateError: null,
};

export const deleteTask = createAsyncThunk(
  "Task/delete",
  async (taskId: number, { rejectWithValue }) => {
    try {
      await axiosInstance.delete(`/tasks/`, {data: {taskId: taskId} });
      return taskId;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (error: any) {
      return rejectWithValue(error.response?.data || "Failed to delete task");
    }
  }
);



export const fetchChangeTaskData = createAsyncThunk(
  "Task/data/change",
  async ({ title, description, status, taskId }: { title: string; status: TaskStatus; taskId: number; description: string }, { rejectWithValue }) => {
    try {
      const response = await axiosInstance.put(`/tasks/`, { title, status, taskId, description });
      return response.data; 
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (error: any) {
      return rejectWithValue(error.response?.data || "Failed to update task data");
    }
  }
);


export const fetchGetAllTasks = createAsyncThunk("tasks/fetchAll", async (sort: { sortBy: string; descending: boolean }) => {
  const response = await axiosInstance.get("/tasks", { params: sort });
  return response.data;
});


export const createTask = createAsyncThunk(
  "tasks/createTask",
  async (taskData: Omit<CreateTask, "id">, { rejectWithValue }) => {
    try {
      const response = await axiosInstance.post("/tasks/", taskData);
      return response.data;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (error: any) {
      return rejectWithValue(error.response?.data || "Failed to create task");
    }
  }
);

const tasksSlice = createSlice({
  name: "tasks",
  initialState,
  reducers: {
    addTaskRealtime: (state, action) => {
      state.tasks.push(action.payload);
    },
    changeTaskRealtime: (state, action) => {
      const index = state.tasks.findIndex(task => task.id === action.payload.id);
      if (index !== -1) {
        state.tasks[index] = action.payload;
      }
    },
    deleteTaskRealtime: (state, action) => {
      state.tasks = state.tasks.filter(task => task.id !== action.payload.id);
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGetAllTasks.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchGetAllTasks.fulfilled, (state, action) => {
        state.tasks = Array.isArray(action.payload.tasks) ? action.payload.tasks : [];
        state.loading = false;
      })
      .addCase(fetchGetAllTasks.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })

      .addCase(fetchChangeTaskData.pending, (state) => {
        state.taskLoading = true;
        state.taskError = null;
      })
      .addCase(fetchChangeTaskData.fulfilled, (state, action) => {
        const index = state.tasks.findIndex(task => task.id === action.payload.id);
        if (index !== -1) {
          state.tasks[index] = action.payload; 
        }
        state.taskLoading = false;
      })
      .addCase(fetchChangeTaskData.rejected, (state, action) => {
        state.taskLoading = false;
        state.taskError = action.payload as string;
      })

      .addCase(deleteTask.fulfilled, (state, action) => {
        state.tasks = state.tasks.filter(task => task.id !== action.payload);
      })
      .addCase(deleteTask.rejected, (state, action) => {
        state.error = action.payload as string;
      });
  },
});

export const { addTaskRealtime, changeTaskRealtime, deleteTaskRealtime } = tasksSlice.actions;
export default tasksSlice.reducer;

