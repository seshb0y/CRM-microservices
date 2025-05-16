import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { deleteUser, fetchChangeUserData } from "../../features/user/userSlice";
import { User } from '../../types/User';
import { AppDispatch } from "../../store/store";
import { OrderStatus } from "../../types/Order";
import { TaskStatus } from "../../types/Task";

interface UserModalProps {
  user: User;
  onClose: () => void;
}

interface FormData {
  username: string;
  email: string;
  userRole: 0 | 1;
}

const UserModal = ({ user, onClose }: UserModalProps) => {
  const dispatch = useDispatch<AppDispatch>();
  const [isEditing, setIsEditing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState<FormData>({
    username: user.username,
    email: user.email,
    userRole: user.userRole === 0 ? 0 : 1
  });

  useEffect(() => {
    setFormData({
      username: user.username,
      email: user.email,
      userRole: user.userRole
    });
  }, [user]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    if (name === 'userRole') {
      const roleValue = Number(value) as 0 | 1;
      setFormData(prev => ({ ...prev, userRole: roleValue }));
    } else {
      setFormData(prev => ({ ...prev, [name]: value }));
    }
  };

  const handleSave = async () => {
    try {
      setError(null);
      await dispatch(fetchChangeUserData({
        username: formData.username,
        newEmail: formData.email,
        oldEmail: user.email,
        role: formData.userRole
      })).unwrap();
      setIsEditing(false);
    } catch (error) {
      setError('Ошибка при обновлении пользователя. Пожалуйста, попробуйте снова.');
      console.error('Error updating user:', error);
    }
  };

  const handleDelete = async () => {
    if (window.confirm("Вы уверены, что хотите удалить этого пользователя?")) {
      try {
        setError(null);
        await dispatch(deleteUser(user.email)).unwrap();
        onClose();
      } catch (error) {
        setError('Ошибка при удалении пользователя. Пожалуйста, попробуйте снова.');
        console.error('Ошибка при удалении пользователя:', error);
      }
    }
  };

  const getStatusColor = (status: OrderStatus | TaskStatus): string => {
    switch (status) {
      case OrderStatus.New:
      case TaskStatus.New:
        return 'bg-blue-500/20 text-blue-300';
      case OrderStatus.Processing:
      case TaskStatus.InProgress:
        return 'bg-yellow-500/20 text-yellow-300';
      case OrderStatus.Completed:
      case TaskStatus.Completed:
        return 'bg-green-500/20 text-green-300';
      default:
        return 'bg-yellow-500/20 text-yellow-300';
    }
  };

  const getStatusText = (status: OrderStatus | TaskStatus): string => {
    return String(status) || "Unknown";
  };

  return (
    <div className="fixed inset-0 flex justify-center items-center bg-black/50 backdrop-blur-sm z-50">
      <div className="bg-gradient-to-br from-[rgba(30,27,75,0.95)] to-[rgba(88,28,135,0.9)] p-8 rounded-lg w-[500px] max-h-[80vh] overflow-auto shadow-xl border border-purple-500/20">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-purple-200 to-purple-400">
            {isEditing ? "Редактировать пользователя" : "Информация о пользователе"}
          </h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-white transition-colors"
          >
            ✕
          </button>
        </div>

        {error && (
          <div className="mb-4 p-3 bg-red-500/10 border border-red-500/20 rounded-lg text-red-300">
            {error}
          </div>
        )}

        <div className="space-y-4">
          <div>
            <label className="block text-sm text-gray-300 mb-1">Имя пользователя</label>
            <input
              type="text"
              name="username"
              value={formData.username}
              onChange={handleChange}
              disabled={!isEditing}
              className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50 disabled:opacity-50"
            />
          </div>

          <div>
            <label className="block text-sm text-gray-300 mb-1">Email</label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              disabled={!isEditing}
              className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50 disabled:opacity-50"
            />
          </div>

          <div>
            <label className="block text-sm text-gray-300 mb-1">Роль</label>
            {isEditing ? (
              <select
                name="userRole"
                value={formData.userRole}
                onChange={handleChange}
                className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
              >
                <option value={0}>Администратор</option>
                <option value={1}>Менеджер</option>
              </select>
            ) : (
              <input
                type="text"
                value={formData.userRole === 0 ? 'Администратор' : 'Менеджер'}
                disabled
                className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50 disabled:opacity-50"
              />
            )}
          </div>

          <div className="flex gap-3 mt-6">
            {isEditing ? (
              <>
                <button
                  onClick={handleSave}
                  className="flex-1 bg-gradient-to-r from-purple-600 to-purple-800 hover:from-purple-700 hover:to-purple-900 px-4 py-2 rounded-lg text-white transition-all duration-300 hover:shadow-lg hover:shadow-purple-500/20"
                >
                  Сохранить
                </button>
                <button
                  onClick={() => setIsEditing(false)}
                  className="flex-1 bg-gradient-to-r from-gray-600 to-gray-800 hover:from-gray-700 hover:to-gray-900 px-4 py-2 rounded-lg text-white transition-all duration-300"
                >
                  Отмена
                </button>
              </>
            ) : (
              <>
                <button
                  onClick={() => setIsEditing(true)}
                  className="flex-1 bg-gradient-to-r from-purple-600 to-purple-800 hover:from-purple-700 hover:to-purple-900 px-4 py-2 rounded-lg text-white transition-all duration-300 hover:shadow-lg hover:shadow-purple-500/20"
                >
                  Редактировать
                </button>
                <button
                  onClick={handleDelete}
                  className="flex-1 bg-gradient-to-r from-red-600 to-red-800 hover:from-red-700 hover:to-red-900 px-4 py-2 rounded-lg text-white transition-all duration-300"
                >
                  Удалить
                </button>
              </>
            )}
          </div>
        </div>

        <h3 className="text-xl font-semibold mt-4">Заказы</h3>
        {user.orders && user.orders.length > 0 ? (
          <ul className="mt-2 space-y-2">
            {user.orders.map(order => (
              <li key={order.orderId} className="p-3 bg-[rgba(30,27,75,0.95)] rounded-lg border border-purple-500/20">
                <p className="text-gray-300">ID: <span className="text-white">#{order.orderId}</span></p>
                <p className="text-gray-300">Бюджет: <span className="text-white">{order.totalAmount ? Number(order.totalAmount).toFixed(2) : '0.00'} $</span></p>
                <p className="text-gray-300">
                  Статус: 
                  <span className={`ml-2 px-2 py-1 rounded-full text-xs ${getStatusColor(order.status)}`}>
                    {getStatusText(order.status)}
                  </span>
                </p>
              </li>
            ))}
          </ul>
        ) : (
          <p className="text-gray-400 mt-2">Нет доступных заказов</p>
        )}

        <h3 className="text-xl font-semibold mt-4">Задачи</h3>
        {user.tasks && user.tasks.length > 0 ? (
          <ul className="mt-2 space-y-2">
            {user.tasks.map(task => (
              <li key={task.taskId} className="p-3 bg-[rgba(30,27,75,0.95)] rounded-lg border border-purple-500/20">
                <p className="text-gray-300">ID: <span className="text-white">#{task.taskId}</span></p>
                <p className="text-gray-300">Название: <span className="text-white">{task.title}</span></p>
                <p className="text-gray-300">
                  Статус: 
                  <span className={`ml-2 px-2 py-1 rounded-full text-xs ${getStatusColor(task.status)}`}>
                    {getStatusText(task.status)}
                  </span>
                </p>
              </li>
            ))}
          </ul>
        ) : (
          <p className="text-gray-400 mt-2">Нет доступных задач</p>
        )}
      </div>
    </div>
  );
};

export default UserModal;
