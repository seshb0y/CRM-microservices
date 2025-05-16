import { Routes, Route, Navigate } from "react-router-dom";
import { useSelector } from "react-redux";
import { RootState } from "../store/store";
import { lazy, Suspense } from "react";
import LoadingPage from "../components/LoadingPage";
import PrivateRoute from "./PrivateRoute";
const Login = lazy(() => import("../pages/Login"));
const Dashboard = lazy(() => import("../pages/Dashboard"));
const Clients = lazy(() => import("../pages/Clients"));
const Orders = lazy(() => import("../pages/Orders"));
const Tasks = lazy(() => import("../pages/Tasks"));
const Users = lazy(() => import("../pages/Users"));
const VerifyEmail = lazy(() => import("../pages/VerifyEmail"));
const DashboardKanban = lazy(() => import("../pages/DashboardKanban"));
const OrderDetailsPage = lazy(() => import("../components/kanban/OrderDetailsPage"));
const ChangePassword = lazy(() => import("../pages/ChangePassword"));

const AppRoutes = () => {
  const isLogin = useSelector((state: RootState) => state.auth.isAuthenticated);

  return (
    <Routes>
      <Route path="/" element={isLogin ? <Navigate to="/dashboard" /> : <Navigate to="/login" />} />
      <Route 
        path="/login" 
        element={
          <Suspense fallback={<LoadingPage />}>
            {isLogin ? <Navigate to="/dashboard" /> : <Login />}
          </Suspense>
        } 
      />
      <Route 
        path="/verify-email" 
        element={
          <Suspense fallback={<LoadingPage />}>
            <VerifyEmail />
          </Suspense>
        } 
      />
      <Route 
        path="/change-password" 
        element={
          <Suspense fallback={<LoadingPage />}>
            <ChangePassword />
          </Suspense>
        } 
      />
      <Route element={<PrivateRoute />}>
        <Route 
          path="/dashboard" 
          element={
            <Suspense fallback={<LoadingPage />}>
              <Dashboard />
            </Suspense>
          } 
        />
        <Route 
          path="/clients" 
          element={
            <Suspense fallback={<LoadingPage />}>
              <Clients />
            </Suspense>
          } 
        />
        <Route 
          path="/tasks" 
          element={
            <Suspense fallback={<LoadingPage />}>
              <Tasks />
            </Suspense>
          } 
        />
        <Route 
          path="/orders" 
          element={
            <Suspense fallback={<LoadingPage />}>
              <Orders />
            </Suspense>
          } 
        />
        <Route 
          path="/kanban" 
          element={
            <Suspense fallback={<LoadingPage />}>
              <DashboardKanban />
            </Suspense>
          } 
        />
        <Route 
          path="/orders/:orderId" 
          element={
            <Suspense fallback={<LoadingPage />}>
              <OrderDetailsPage />
            </Suspense>
          } 
        />
        <Route 
          path="/users" 
          element={
            <Suspense fallback={<LoadingPage />}>
              <Users />
            </Suspense>
          } 
        />
      </Route>

      <Route path="*" element={isLogin ? <Navigate to="/dashboard" /> : <Navigate to="/login" />} />
    </Routes>
  );
};

export default AppRoutes;
