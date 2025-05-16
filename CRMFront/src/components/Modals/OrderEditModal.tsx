import { useState, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Order, OrderStatus } from '../../types/Order';
import { fetchChangeOrderData } from '../../features/orders/orderSlice';
import { toast } from 'react-toastify';
import { AppDispatch, RootState } from '../../store/store';
import { fetchAssignUserToOrder } from '../../features/orders/orderSlice';
import { fetchUsers } from '../../features/user/userSlice';

interface OrderEditModalProps {
  order: Order;
  onClose: () => void;
  onUpdate: (updatedOrder: Order) => void;
}

const OrderEditModal = ({ order, onClose, onUpdate }: OrderEditModalProps) => {
  const dispatch = useDispatch<AppDispatch>();
  const { users } = useSelector((state: RootState) => state.users);
  const [editedOrder, setEditedOrder] = useState<Order>(order);
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);

  useEffect(() => {
    dispatch(fetchUsers({}));
  }, [dispatch]);

  const getStatusNumber = (status: OrderStatus): number => {
    switch (status) {
      case OrderStatus.New:
        return 0;
      case OrderStatus.Processing:
        return 1;
      case OrderStatus.Completed:
        return 2;
      default:
        return 0;
    }
  };

  const handleSave = async () => {
    if (!editedOrder.id) {
      toast.error('ID заказа не определен');
      return;
    }

    try {
      const request = {
        totalAmount: Number(editedOrder.totalAmount),
        status: getStatusNumber(editedOrder.status),
        orderId: editedOrder.id
      };

      await dispatch(fetchChangeOrderData(request)).unwrap();

      if (selectedUserId) {
        await dispatch(fetchAssignUserToOrder({
          orderId: editedOrder.id,
          userId: Number(selectedUserId)
        })).unwrap();
      }

      onUpdate(editedOrder);
      onClose();
      toast.success('Заказ успешно обновлен');
    } catch (error) {
      console.error('Ошибка при обновлении заказа:', error);
      toast.error('Ошибка при обновлении заказа');
    }
  };

  return (
    <div className="fixed inset-0 flex justify-center items-center bg-black/50 backdrop-blur-sm z-50">
      <div className="bg-gradient-to-br from-[rgba(30,27,75,0.95)] to-[rgba(88,28,135,0.9)] p-8 rounded-lg w-[500px] max-h-[80vh] overflow-auto shadow-xl border border-purple-500/20">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-purple-200 to-purple-400">
            Редактировать заказ
          </h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-white transition-colors"
          >
            ✕
          </button>
        </div>

        <div className="space-y-4">
          <div>
            <label className="block text-sm text-gray-300 mb-1">Статус</label>
            <select
              value={editedOrder.status}
              onChange={(e) => setEditedOrder({ ...editedOrder, status: e.target.value as OrderStatus })}
              className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
            >
              {Object.values(OrderStatus).map((status) => (
                <option key={status} value={status}>
                  {status}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm text-gray-300 mb-1">Сумма</label>
            <input
              type="number"
              value={editedOrder.totalAmount}
              onChange={(e) => setEditedOrder({ ...editedOrder, totalAmount: Number(e.target.value) })}
              className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
            />
          </div>

          <div>
            <label className="block text-sm text-gray-300 mb-1">Назначить пользователя</label>
            <select
              value={selectedUserId || ''}
              onChange={(e) => setSelectedUserId(e.target.value || null)}
              className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
            >
              <option value="">Выберите пользователя</option>
              {users.map((user) => (
                <option key={user.userId} value={user.userId}>
                  {user.username}
                </option>
              ))}
            </select>
          </div>

          <div className="flex gap-3 mt-6">
            <button
              onClick={handleSave}
              className="flex-1 bg-gradient-to-r from-purple-600 to-purple-800 hover:from-purple-700 hover:to-purple-900 px-4 py-2 rounded-lg text-white transition-all duration-300 hover:shadow-lg hover:shadow-purple-500/20"
            >
              Сохранить
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

export default OrderEditModal; 