import { useEffect } from 'react';
import { useAppDispatch } from './hooks/useAppDispatch';
import { checkAuth } from './features/auth/authSlice';
import { useSignalR } from './hooks/useSignalR';
import AppRoutes from './routes/AppRoutes';

const AppInitializer = () => {
  const dispatch = useAppDispatch();

  useEffect(() => {
    if (localStorage.getItem("isLogin")) {
      dispatch(checkAuth());
    }
  }, [dispatch]);

  useSignalR();

  return <AppRoutes />;
};

export default AppInitializer;
