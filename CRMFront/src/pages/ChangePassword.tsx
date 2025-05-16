import { useState, ChangeEvent } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { toast } from "sonner";
import Lottie from "lottie-react";
import eyeAnimation from "../assets/Login.json";
import axiosInstance from "../api/axiosInstance";

const ChangePassword = () => {
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token");
  const navigate = useNavigate();
  
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async () => {
    if (!token) {
      toast.error("Password reset token is missing");
      return;
    }

    if (newPassword !== confirmPassword) {
      toast.error("Passwords do not match");
      return;
    }

    if (newPassword.length < 6) {
      toast.error("Password must be at least 6 characters long");
      return;
    }

    try {
      setIsLoading(true);
      await axiosInstance.post("/account/password/change", {
        newPassword,
        token
      });
      
      toast.success("Password successfully changed");
      navigate("/");
    } catch {
      toast.error("Error changing password. The link may be expired or invalid");
    } finally {
      setIsLoading(false);
    }
  };

  if (!token) {
    return (
      <div className="h-screen w-screen flex flex-col items-center justify-center bg-gradient-to-br from-indigo-950 via-purple-950 to-slate-900 text-white">
        <div className="bg-[#1a0b2e]/90 p-8 rounded-xl border border-[#5a2d82]/30 shadow-xl backdrop-blur-sm">
          <h2 className="text-xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-purple-200 to-purple-400 mb-4">Error</h2>
          <p className="text-gray-400">Invalid password reset link</p>
        </div>
      </div>
    );
  }

  return (
    <div className="h-screen w-screen flex flex-col items-center justify-center bg-gradient-to-br from-indigo-950 via-purple-950 to-slate-900 text-white">
      <div className="relative w-40 h-40 mb-8">
        <div className="absolute inset-0 bg-primary-purple/20 blur-3xl rounded-full" />
        <div className="relative z-10">
          <Lottie animationData={eyeAnimation} loop={true} />
        </div>
      </div>

      <div className="bg-[#1a0b2e]/90 p-8 rounded-xl shadow-lg w-96 backdrop-blur-sm border border-[#5a2d82]/30">
        <h2 className="text-2xl font-bold mb-6 text-center bg-clip-text text-transparent bg-gradient-to-r from-purple-200 to-purple-400">
          Change Password
        </h2>

        <div className="mb-4">
          <label className="block text-gray-300 mb-2 text-sm">New Password</label>
          <input
            type="password"
            placeholder="••••••••"
            value={newPassword}
            onChange={(e: ChangeEvent<HTMLInputElement>) => setNewPassword(e.target.value)}
            className="w-full p-3 rounded-lg bg-[#2a1042] border border-[#5a2d82]/50 focus:outline-none focus:border-primary-purple text-white placeholder-gray-500"
          />
        </div>

        <div className="mb-6">
          <label className="block text-gray-300 mb-2 text-sm">Confirm Password</label>
          <input
            type="password"
            placeholder="••••••••"
            value={confirmPassword}
            onChange={(e: ChangeEvent<HTMLInputElement>) => setConfirmPassword(e.target.value)}
            className="w-full p-3 rounded-lg bg-[#2a1042] border border-[#5a2d82]/50 focus:outline-none focus:border-primary-purple text-white placeholder-gray-500"
          />
        </div>

        <button
          onClick={handleSubmit}
          disabled={isLoading}
          className="w-full bg-primary-purple hover:bg-purple-700 text-white font-semibold py-3 rounded-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-primary-purple/20"
        >
          {isLoading ? (
            <div className="flex items-center justify-center gap-2">
              <div className="w-5 h-5 border-2 border-white/20 border-t-white rounded-full animate-spin" />
              <span>Saving...</span>
            </div>
          ) : (
            "Save new password"
          )}
        </button>
      </div>
    </div>
  );
};

export default ChangePassword; 