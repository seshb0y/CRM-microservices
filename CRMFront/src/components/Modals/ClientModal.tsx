import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { RootState, AppDispatch } from "../../store/store";
import { deleteClient, fetchChangeClientData } from "../../features/clients/clientSlice";
import { createOrder } from "../../features/orders/orderSlice";
import { Client } from "../../types/Client";
import { checkAuth } from "../../features/auth/authSlice";
import { fetchUsers } from "../../features/user/userSlice";

interface ClientModalProps {
  client: Client;
  onClose: () => void;
}

const ClientModal = ({ client, onClose }: ClientModalProps) => {
  const dispatch = useDispatch<AppDispatch>();
  const { clientLoading, clientError, clients } = useSelector((state: RootState) => state.clients);
  const { user } = useSelector((state: RootState) => state.auth);
  const { users } = useSelector((state: RootState) => state.users);
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState<Client>(client);
  const [currentClient, setCurrentClient] = useState<Client>(client);

  const [newOrderData, setNewOrderData] = useState({
    totalAmount: "",
    userId: ""
  });

  useEffect(() => {
    dispatch(fetchUsers({}));
  }, [dispatch]);

  useEffect(() => {
    const updatedClient = clients.find((c) => c.id === client.id);
    if (updatedClient) {
      setFormData(updatedClient);
      setCurrentClient(updatedClient);
      if (isEditing) {
        setIsEditing(false);
      }
    }
  }, [clients, client.id]);

  const formatPhoneNumber = (value: string) => {
    const phoneNumber = value.replace(/\D/g, '');
    
    const hasPlus = value.startsWith('+');
    
    let countryCode = '';
    let localNumber = '';
    
    if (phoneNumber.length > 0) {
      if (phoneNumber.length >= 3) {
        countryCode = phoneNumber.slice(0, 3);
        localNumber = phoneNumber.slice(3);
      } else {
        countryCode = phoneNumber;
      }
    }
    
    let formatted = hasPlus ? '+' : '';
    if (countryCode) {
      formatted += countryCode;
      if (localNumber) {
        formatted += ' ' + localNumber;
      }
    }
    
    return formatted;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    
    if (name === 'phone') {
      setFormData(prev => ({
        ...prev,
        [name]: formatPhoneNumber(value)
      }));
    } else {
      setFormData(prev => ({
        ...prev,
        [name]: value
      }));
    }
  };

  const handleSave = async () => {
    const phoneForServer = formData.phone.replace(/\D/g, '');
    
    try {
      const result = await dispatch(
        fetchChangeClientData({
          name: formData.name,
          newEmail: formData.email,
          oldEmail: currentClient.email,
          phone: phoneForServer,
          address: formData.address,
        })
      ).unwrap();
      
      setFormData(result);
      setCurrentClient(result);
      setIsEditing(false);
    } catch (error) {
      console.error('Error updating client:', error);
    }
  };

  const handleDelete = () => {
    if (window.confirm("Are you sure you want to delete this client?")) {
      dispatch(deleteClient(client.email));
      onClose();
    }
  };

  const handleCreateOrder = async () => {
    if (!newOrderData.totalAmount) {
      alert("Введите сумму заказа");
      return;
    }

    if (!newOrderData.userId) {
      alert("Выберите ответственного");
      return;
    }
  
    try {
      const authResponse = await dispatch(checkAuth()).unwrap();
      const updatedUserEmail = authResponse?.email || user?.email || "";

  
      dispatch(
        createOrder({
          totalAmount: parseFloat(newOrderData.totalAmount),
          clientEmail: client.email,
          userEmail: updatedUserEmail,
          userId: Number(newOrderData.userId)
        })
      );
  
      setNewOrderData({ totalAmount: "", userId: "" });
  
    } catch (error) {
      console.error("Ошибка при обновлении пользователя:", error);
    }
  };
  
  return (
    <div className="fixed inset-0 flex justify-center items-center bg-black/50 backdrop-blur-sm">
      <div className="bg-gradient-to-br from-[rgba(30,27,75,0.95)] to-[rgba(88,28,135,0.9)] p-8 rounded-xl w-[600px] max-h-[90vh] overflow-auto shadow-xl border border-purple-500/10">
        <h2 className="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-purple-200 to-purple-400 mb-6">
          {isEditing ? "Редактирование клиента" : "Информация о клиенте"}
        </h2>

        {clientLoading && (
          <div className="flex items-center gap-2 text-purple-300 mb-4">
            <div className="w-5 h-5 border-2 border-purple-400/20 rounded-full animate-spin border-t-purple-400"></div>
            Загрузка данных...
          </div>
        )}
        
        {clientError && (
          <div className="bg-red-500/10 text-red-400 p-3 rounded-lg mb-4">
            {clientError}
          </div>
        )}

        <div className="space-y-4">
          {["name", "email", "phone", "address"].map((field) => (
            <div key={field}>
              <label className="block text-sm text-purple-200 mb-1 font-medium">
                {field.charAt(0).toUpperCase() + field.slice(1)}
              </label>
              <input
                type={field === "email" ? "email" : "text"}
                name={field}
                value={String(formData[field as keyof Client] ?? "")}
                disabled={!isEditing}
                onChange={handleChange}
                className={`w-full px-4 py-2.5 rounded-lg border text-white transition-colors ${
                  isEditing 
                    ? "bg-[rgba(30,27,75,0.5)] border-purple-500/10 focus:outline-none focus:border-purple-500/30" 
                    : "bg-[rgba(30,27,75,0.3)] border-transparent cursor-not-allowed"
                }`}
                placeholder={field === "phone" ? "+7 (___) ___-__-__" : `Введите ${field}`}
              />
            </div>
          ))}
        </div>

        <div className="border-t border-purple-500/10 my-6"></div>

        <div className="space-y-4 mb-6">
          <h3 className="text-lg font-medium text-purple-200">Создать заказ</h3>
          
          <div>
            <label className="block text-sm text-purple-200 mb-1 font-medium">Сумма заказа</label>
            <input
              type="text"
              name="totalAmount"
              value={newOrderData.totalAmount}
              onChange={(e) => setNewOrderData({ ...newOrderData, totalAmount: e.target.value })}
              className="w-full px-4 py-2.5 rounded-lg bg-[rgba(30,27,75,0.5)] border border-purple-500/10 text-white placeholder-purple-300/30 focus:outline-none focus:border-purple-500/30 transition-colors"
              placeholder="Введите сумму заказа"
            />
          </div>

          <div>
            <label className="block text-sm text-purple-200 mb-1 font-medium">Ответственный</label>
            <select
              name="userId"
              value={newOrderData.userId}
              onChange={(e) => setNewOrderData({ ...newOrderData, userId: e.target.value })}
              className="w-full px-4 py-2.5 rounded-lg bg-[rgba(30,27,75,0.5)] border border-purple-500/10 text-white focus:outline-none focus:border-purple-500/30 transition-colors"
            >
              <option value="">Выберите ответственного</option>
              {users.map((user) => (
                <option key={user.userId} value={user.userId} className="bg-[rgba(30,27,75,0.95)]">
                  {user.username}
                </option>
              ))}
            </select>
          </div>

          <button 
            className="w-full bg-gradient-to-r from-purple-600 to-purple-800 hover:from-purple-700 hover:to-purple-900 px-6 py-2.5 rounded-lg text-white font-medium transition-all duration-300 hover:shadow-lg hover:shadow-purple-500/20"
            onClick={handleCreateOrder}
          >
            Создать заказ
          </button>
        </div>

        <div className="flex flex-col gap-3">
          <button
            className="w-full bg-gradient-to-r from-purple-600 to-purple-800 hover:from-purple-700 hover:to-purple-900 px-6 py-2.5 rounded-lg text-white font-medium transition-all duration-300 hover:shadow-lg hover:shadow-purple-500/20"
            onClick={isEditing ? handleSave : () => setIsEditing(true)}
          >
            {isEditing ? "Сохранить" : "Редактировать"}
          </button>

          <button
            className="w-full bg-gradient-to-r from-red-600 to-red-800 hover:from-red-700 hover:to-red-900 px-6 py-2.5 rounded-lg text-white font-medium transition-all duration-300 hover:shadow-lg hover:shadow-red-500/20"
            onClick={handleDelete}
          >
            Удалить клиента
          </button>

          <button 
            className="w-full bg-[rgba(30,27,75,0.5)] hover:bg-[rgba(30,27,75,0.7)] px-6 py-2.5 rounded-lg text-white/80 font-medium transition-colors border border-purple-500/10 hover:border-purple-500/20"
            onClick={onClose}
          >
            Закрыть
          </button>
        </div>
      </div>
    </div>
  );
};

export default ClientModal;
