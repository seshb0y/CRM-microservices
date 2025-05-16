import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axiosInstance from "../../api/axiosInstance";
import { User } from "../../types/User";
import { ResetPasswordRequest } from "../../types/auth";

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  loading: boolean;
}

// interface User {
//   id: string,
//   name: string,
//   role: string,
// };

export const checkAuth = createAsyncThunk("auth/checkAuth", async () => {
  const response = await axiosInstance.get("/account/me");
  return response.data;
});


export const loginUser = createAsyncThunk<User,{ username: string; password: string }>(
  "auth/login",
  async ({ username, password }) => {
    const response = await axiosInstance.post("/auth/login", { username, password });

    return response.data;
  }
);


export const logoutUser = createAsyncThunk("auth/logout", async () => {
  await axiosInstance.post("/auth/logout");
});

export const resetPassword = createAsyncThunk(
  "auth/resetPassword",
  async (request: ResetPasswordRequest) => {
    const response = await axiosInstance.post("/account/password/reset", request);
    return response.data;
  }
);

const initialState: AuthState = {
  user: null,
  isAuthenticated: localStorage.getItem("isLogin") === "true",
  loading: true,
};





const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(checkAuth.pending, (state) => {
        state.loading = true;
      })
      .addCase(checkAuth.fulfilled, (state, action) => {
        state.isAuthenticated = true;
        state.user = action.payload;
        state.loading = false;
        localStorage.setItem("isLogin", "true");
      })
      .addCase(checkAuth.rejected, (state) => {
        state.isAuthenticated = false;
        state.user = null;
        state.loading = false;
        localStorage.removeItem("isLogin");
      })
      .addCase(loginUser.pending, (state) => {
        state.loading = true;
      })
      .addCase(loginUser.fulfilled, (state, action) => {
        state.isAuthenticated = true;
        state.user = action.payload;
        state.loading = false;
        localStorage.setItem("isLogin", "true");
      })
      .addCase(loginUser.rejected, (state) => {
        state.isAuthenticated = false;
        state.user = null;
        state.loading = false;
        localStorage.removeItem("isLogin");
      })
      .addCase(logoutUser.fulfilled, (state) => {
        state.isAuthenticated = false;
        state.user = null;
        state.loading = false;
        localStorage.removeItem("isLogin");
      });
  },
});

export default authSlice.reducer;
