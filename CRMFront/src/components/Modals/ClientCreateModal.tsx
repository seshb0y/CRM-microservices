import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { createClient } from "../../features/clients/clientSlice";
import { RootState, AppDispatch } from "../../store/store";

interface ClientCreateModalProps {
  onClose: () => void;
}

const ClientCreateModal = ({ onClose }: ClientCreateModalProps) => {
  const dispatch = useDispatch<AppDispatch>();
  const { clientCreating, clientCreateError } = useSelector((state: RootState) => state.clients);

  const [formData, setFormData] = useState({
    name: "",
    email: "",
    phone: "",
    address: "",
    createdAt: new Date().toISOString(),
  });

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

  const handleSave = () => {
    const phoneForServer = formData.phone.replace(/\D/g, '');
    
    dispatch(createClient({
      ...formData,
      phone: phoneForServer
    }));
    onClose();
  };

  return (
    <div className="fixed inset-0 flex justify-center items-center bg-black/50 backdrop-blur-sm">
      <div className="bg-gradient-to-br from-[rgba(30,27,75,0.95)] to-[rgba(88,28,135,0.9)] p-8 rounded-xl w-[500px] shadow-xl border border-purple-500/10">
        <h2 className="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-purple-200 to-purple-400 mb-6">Новый клиент</h2>

        {clientCreating && (
          <div className="flex items-center gap-2 text-purple-300 mb-4">
            <div className="w-5 h-5 border-2 border-purple-400/20 rounded-full animate-spin border-t-purple-400"></div>
            Создание клиента...
          </div>
        )}
        
        {clientCreateError && (
          <div className="bg-red-500/10 text-red-400 p-3 rounded-lg mb-4">
            {clientCreateError}
          </div>
        )}

        {["name", "email", "phone", "address"].map((field) => (
          <div key={field} className="mb-4">
            <label className="block text-sm text-purple-200 mb-1 font-medium">
              {field.charAt(0).toUpperCase() + field.slice(1)}
            </label>
            <input
              type={field === "email" ? "email" : "text"}
              name={field}
              value={formData[field as keyof typeof formData]}
              onChange={handleChange}
              className="w-full px-4 py-2.5 rounded-lg bg-[rgba(30,27,75,0.5)] border border-purple-500/10 text-white placeholder-purple-300/30 focus:outline-none focus:border-purple-500/30 transition-colors"
              placeholder={field === "phone" ? "+7 (___) ___-__-__" : `Введите ${field}`}
            />
          </div>
        ))}

        <div className="flex gap-3 mt-6">
          <button 
            className="flex-1 bg-gradient-to-r from-purple-600 to-purple-800 hover:from-purple-700 hover:to-purple-900 px-6 py-2.5 rounded-lg text-white font-medium transition-all duration-300 hover:shadow-lg hover:shadow-purple-500/20"
            onClick={handleSave}
          >
            Сохранить
          </button>
          <button 
            className="flex-1 bg-[rgba(30,27,75,0.5)] hover:bg-[rgba(30,27,75,0.7)] px-6 py-2.5 rounded-lg text-white/80 font-medium transition-colors border border-purple-500/10 hover:border-purple-500/20"
            onClick={onClose}
          >
            Отмена
          </button>
        </div>
      </div>
    </div>
  );
};

export default ClientCreateModal;
