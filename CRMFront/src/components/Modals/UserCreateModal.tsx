import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { createUser } from "../../features/user/userSlice";
import { RootState } from "../../store/store";

interface UserCreateModalProps {
  onClose: () => void;
}

const UserCreateModal = ({ onClose }: UserCreateModalProps) => {
  const dispatch = useDispatch();
  const { userCreating, userCreateError } = useSelector((state: RootState) => state.users);

  const [formData, setFormData] = useState({
    username: "",
    email: "",
    password: "",
    confirmPassword: "",
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSave = () => {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    dispatch(createUser(formData) as any);
    onClose();
  };

  return (
    <div className="fixed inset-0 flex justify-center items-center bg-black/50 backdrop-blur-sm z-50">
      <div className="bg-gradient-to-br from-[rgba(30,27,75,0.95)] to-[rgba(88,28,135,0.9)] p-8 rounded-lg w-[500px] max-h-[80vh] overflow-auto shadow-xl border border-purple-500/20">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-purple-200 to-purple-400">
            Создать пользователя
          </h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-white transition-colors"
          >
            ✕
          </button>
        </div>

        <div className="space-y-4">
          {userCreating && (
            <div className="bg-blue-500/20 text-blue-300 p-3 rounded-lg">
              Создание пользователя...
            </div>
          )}
          {userCreateError && (
            <div className="bg-red-500/20 text-red-300 p-3 rounded-lg">
              {userCreateError}
            </div>
          )}

          <div>
            <label className="block text-sm text-gray-300 mb-1">Имя пользователя</label>
            <input
              type="text"
              name="username"
              value={formData.username}
              onChange={handleChange}
              className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
              placeholder="Введите имя пользователя"
            />
          </div>

          <div>
            <label className="block text-sm text-gray-300 mb-1">Email</label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
              placeholder="Введите email"
            />
          </div>

          <div>
            <label className="block text-sm text-gray-300 mb-1">Пароль</label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
              placeholder="Введите пароль"
            />
          </div>

          <div>
            <label className="block text-sm text-gray-300 mb-1">Подтверждение пароля</label>
            <input
              type="password"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
              placeholder="Подтвердите пароль"
            />
          </div>

          <div className="flex gap-3 mt-6">
            <button
              onClick={handleSave}
              className="flex-1 bg-gradient-to-r from-purple-600 to-purple-800 hover:from-purple-700 hover:to-purple-900 px-4 py-2 rounded-lg text-white transition-all duration-300 hover:shadow-lg hover:shadow-purple-500/20"
            >
              Создать
            </button>
            <button
              onClick={onClose}
              className="flex-1 bg-gradient-to-r from-gray-600 to-gray-800 hover:from-gray-700 hover:to-gray-900 px-4 py-2 rounded-lg text-white transition-all duration-300"
            >
              Отмена
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default UserCreateModal;
