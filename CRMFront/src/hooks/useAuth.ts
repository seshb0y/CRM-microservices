import { useSelector } from "react-redux";
import { RootState } from "../store/store";

export const useAuth = () => {
  const { isAuthenticated, user, loading } = useSelector((state: RootState) => state.auth);
  return { isAuthenticated, user, loading };
};
