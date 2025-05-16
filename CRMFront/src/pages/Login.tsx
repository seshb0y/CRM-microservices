import { useState, ChangeEvent } from "react";
import { loginUser, checkAuth, resetPassword } from "../features/auth/authSlice"
import { useNavigate } from "react-router-dom";
import { useAppDispatch } from "../hooks/useAppDispatch";
import Lottie from "lottie-react";
import eyeAnimation from "../assets/Login.json";
import * as Dialog from "@radix-ui/react-dialog";
import { toast } from "sonner";

const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [isResetModalOpen, setIsResetModalOpen] = useState(false);
  const [resetUsername, setResetUsername] = useState("");
  const [isSuccessModalOpen, setIsSuccessModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const handleLogin = async () => {
    try {
      setIsLoading(true);
      await dispatch(loginUser({ username, password })).unwrap();
      await dispatch(checkAuth());
      navigate("/dashboard");
    } catch {
      toast.error("Invalid login credentials");
    } finally {
      setIsLoading(false);
    }
  };

  const handleResetPassword = async () => {
    try {
      setIsLoading(true);
      await dispatch(resetPassword({ username: resetUsername })).unwrap();
      setIsResetModalOpen(false);
      setIsSuccessModalOpen(true);
    } catch {
      toast.error("Error sending password reset request");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="h-screen w-screen flex flex-col items-center justify-center bg-gradient-to-br from-indigo-950 via-purple-950 to-slate-900 text-white">
      <div className="relative w-40 h-40 mb-8">
        <div className="absolute inset-0 bg-primary-purple/20 blur-3xl rounded-full" />
        <div className="relative z-10">
          <Lottie animationData={eyeAnimation} loop={true} />
        </div>
      </div>

      <div className="bg-[#1a0b2e]/90 p-8 rounded-xl shadow-lg w-96 backdrop-blur-sm border border-[#5a2d82]/30">
        <h2 className="text-3xl font-bold mb-6 text-center bg-clip-text text-transparent bg-gradient-to-r from-purple-200 to-purple-400">
          Sign In
        </h2>

        <div className="mb-4">
          <label className="block text-gray-300 mb-2 text-sm">Username</label>
          <input
            type="text"
            placeholder="Enter username"
            value={username}
            onChange={(e: ChangeEvent<HTMLInputElement>) => setUsername(e.target.value)}
            className="w-full p-3 rounded-lg bg-[#2a1042] border border-[#5a2d82]/50 focus:outline-none focus:border-primary-purple text-white placeholder-gray-500"
          />
        </div>

        <div className="mb-6">
          <label className="block text-gray-300 mb-2 text-sm">Password</label>
          <input
            type="password"
            placeholder="Enter password"
            value={password}
            onChange={(e: ChangeEvent<HTMLInputElement>) => setPassword(e.target.value)}
            className="w-full p-3 rounded-lg bg-[#2a1042] border border-[#5a2d82]/50 focus:outline-none focus:border-primary-purple text-white placeholder-gray-500"
          />
        </div>

        <button
          onClick={handleLogin}
          disabled={isLoading}
          className="w-full bg-primary-purple hover:bg-purple-700 text-white font-semibold py-3 rounded-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-primary-purple/20"
        >
          {isLoading ? (
            <div className="flex items-center justify-center gap-2">
              <div className="w-5 h-5 border-2 border-white/20 border-t-white rounded-full animate-spin" />
              <span>Signing in...</span>
            </div>
          ) : (
            "Sign In"
          )}
        </button>

        <button
          onClick={() => setIsResetModalOpen(true)}
          className="w-full mt-4 text-gray-400 hover:text-white transition-colors text-sm"
        >
          Forgot password?
        </button>
      </div>

      <Dialog.Root open={isResetModalOpen} onOpenChange={setIsResetModalOpen}>
        <Dialog.Portal>
          <Dialog.Overlay className="fixed inset-0 bg-black/50 backdrop-blur-sm" />
          <Dialog.Content className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 bg-[#1a0b2e]/95 p-6 rounded-xl w-96 border border-[#5a2d82]/30 shadow-xl">
            <Dialog.Title className="text-2xl font-bold mb-4 bg-clip-text text-transparent bg-gradient-to-r from-purple-200 to-purple-400">
              Reset Password
            </Dialog.Title>
            <Dialog.Description className="text-gray-400 mb-6">
              Enter your username to receive password reset instructions
            </Dialog.Description>
            <input
              type="text"
              placeholder="Enter username"
              value={resetUsername}
              onChange={(e: ChangeEvent<HTMLInputElement>) => setResetUsername(e.target.value)}
              className="w-full p-3 rounded-lg bg-[#2a1042] border border-[#5a2d82]/50 focus:outline-none focus:border-primary-purple text-white placeholder-gray-500 mb-6"
            />
            <div className="flex justify-end gap-3">
              <button
                onClick={() => setIsResetModalOpen(false)}
                className="px-4 py-2 rounded-lg border border-[#5a2d82]/50 hover:bg-[#2a1042] transition-colors text-gray-400 hover:text-white"
              >
                Cancel
              </button>
              <button
                onClick={handleResetPassword}
                disabled={isLoading}
                className="px-6 py-2 rounded-lg bg-primary-purple hover:bg-purple-700 transition-colors text-white font-medium disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-primary-purple/20"
              >
                {isLoading ? (
                  <div className="flex items-center justify-center gap-2">
                    <div className="w-4 h-4 border-2 border-white/20 border-t-white rounded-full animate-spin" />
                    <span>Sending...</span>
                  </div>
                ) : (
                  "Send"
                )}
              </button>
            </div>
          </Dialog.Content>
        </Dialog.Portal>
      </Dialog.Root>

      <Dialog.Root open={isSuccessModalOpen} onOpenChange={setIsSuccessModalOpen}>
        <Dialog.Portal>
          <Dialog.Overlay className="fixed inset-0 bg-black/50 backdrop-blur-sm" />
          <Dialog.Content className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 bg-[#1a0b2e]/95 p-6 rounded-xl w-96 border border-[#5a2d82]/30 shadow-xl">
            <Dialog.Title className="text-2xl font-bold mb-4 bg-clip-text text-transparent bg-gradient-to-r from-purple-200 to-purple-400">
              Success!
            </Dialog.Title>
            <Dialog.Description className="text-gray-400 mb-6">
              Password reset instructions have been sent to your email
            </Dialog.Description>
            <div className="flex justify-end">
              <button
                onClick={() => setIsSuccessModalOpen(false)}
                className="px-6 py-2 rounded-lg bg-primary-purple hover:bg-purple-700 transition-colors text-white font-medium shadow-lg shadow-primary-purple/20"
              >
                OK
              </button>
            </div>
          </Dialog.Content>
        </Dialog.Portal>
      </Dialog.Root>
    </div>
  );
};

export default Login;
