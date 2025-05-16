import { useState, useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import { RootState } from '../../store/store';
import { Order } from '../../types/Order';

interface OrderSearchProps {
  onOrderSelect: (order: Order) => void;
}

export const OrderSearch = ({ onOrderSelect }: OrderSearchProps) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [isOpen, setIsOpen] = useState(false);
  const wrapperRef = useRef<HTMLDivElement>(null);
  const orders = useSelector((state: RootState) => state.orders.orders);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (wrapperRef.current && !wrapperRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const filteredOrders = searchTerm.trim() === '' ? [] : orders.filter(order => 
    (order.id?.toString() || '').includes(searchTerm.toLowerCase()) ||
    (order.username || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
    (order.clientName || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
    order.status.toString().toLowerCase().includes(searchTerm.toLowerCase()) ||
    order.totalAmount.toString().includes(searchTerm)
  );

  const handleSelect = (order: Order) => {
    onOrderSelect(order);
    setSearchTerm('');
    setIsOpen(false);
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('ru-RU', {
      style: 'currency',
      currency: 'RUB',
      minimumFractionDigits: 2
    }).format(price);
  };

  const getStatusText = (status: number): string => {
    switch (status) {
      case 0:
        return "New";
      case 1:
        return "InProgress";
      case 2:
        return "Completed";
      default:
        return "Cancelled";
    }
  };

  return (
    <div ref={wrapperRef} className="relative">
      <div className="relative">
        <input
          type="text"
          value={searchTerm}
          onChange={(e) => {
            setSearchTerm(e.target.value);
            setIsOpen(true);
          }}
          placeholder="Поиск заказов..."
          className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
        />
      </div>

      {isOpen && filteredOrders.length > 0 && (
        <div className="absolute z-50 w-full mt-2 bg-[#2a1042] rounded-lg shadow-xl max-h-60 overflow-y-auto">
          {filteredOrders.map((order) => (
            <div
              key={order.id}
              onClick={() => handleSelect(order)}
              className="px-4 py-2 hover:bg-[#3a1a5e] cursor-pointer transition-colors"
            >
              <div className="flex justify-between items-center">
                <div>
                  <div className="font-medium">Заказ #{order.id}</div>
                  <div className="text-sm text-gray-400">{order.username}</div>
                  <div className="text-sm text-purple-400">{order.clientName || 'Нет клиента'}</div>
                </div>
                <div className="text-right">
                  <div className="font-medium text-purple-400">{formatPrice(order.totalAmount)}</div>
                  <div className="text-sm text-gray-400">{getStatusText(Number(order.status))}</div>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}; 