import { Client } from "../../types/Client";
import { Card, CardHeader, CardTitle, CardContent } from "../ui/card";
import { Badge } from "../ui/badge";
import { useNavigate } from "react-router-dom";
import { OrderStatus } from "../../types/Order";
import { useState } from "react";


const getOrderStatusClass = (status: string) => {
  switch (status) {
    case OrderStatus.New:
      return "bg-blue-500/80 text-white";
    case OrderStatus.Processing:
      return "bg-yellow-500/80 text-white";
    case OrderStatus.Completed:
      return "bg-green-500/80 text-white";
    default:
      return "bg-gray-500/80 text-white";
  }
};

interface CardProps {
  client: Client;
}

const ClientPreviewCard = ({ client }: CardProps) => {
  const navigate = useNavigate();
  const [isDragging, setIsDragging] = useState(false);

  const handleDragStart = (e: React.DragEvent<HTMLDivElement>, orderId: string, currentStatus: OrderStatus) => {
    e.dataTransfer.setData("orderId", orderId);
    e.dataTransfer.setData("currentStatus", currentStatus);
    setIsDragging(true);
  };

  const handleDragEnd = () => {
    setIsDragging(false);
  };

  return (
    <Card className={`bg-[#1a0b2e]/90 text-white border border-[#5a2d82] shadow-lg p-4 card-hover-effect backdrop-blur-sm ${isDragging ? 'dragging' : ''}`}>
      <CardHeader>
        <CardTitle className="text-lg font-semibold flex items-center gap-2">
          {client.name}
          <Badge variant="outline" className="text-xs border-primary-purple text-primary-purple">
            {client.orders?.length || 0} {client.orders?.length === 1 ? 'order' : 'orders'}
          </Badge>
        </CardTitle>
        <p className="text-gray-400 text-sm">{client.email}</p>
        <p className="text-gray-400 text-sm">{client.phone}</p>
      </CardHeader>
      <CardContent>
        <div className="space-y-2 text-sm">
          <div className="text-gray-300 flex gap-2">
            <span className="text-primary-purple min-w-[90px]">Адрес:</span>
            <span className="break-words flex-1">{client.address}</span>
          </div>
          <div className="text-gray-300 flex gap-2">
            <span className="text-primary-purple min-w-[90px]">Регистрация:</span>
            <span>{new Date(client.createdAt).toLocaleDateString()}</span>
          </div>
        </div>

        <div className="mt-4">
          <h3 className="text-md font-medium text-primary-purple mb-3">Заказы:</h3>
          <div className="space-y-3">
            {client.orders != null && client.orders.length > 0 ? (
              client.orders?.map((order) => (
                <div
                  key={order.orderId}
                  className="p-3 bg-[#2a1042]/80 rounded-lg cursor-pointer hover:bg-[#3a1f5a] transition-all duration-300 backdrop-blur-sm border border-gray-800/50"
                  onClick={() => navigate(`/orders/${order.orderId}`)}
                  draggable
                  onDragStart={(e) => handleDragStart(e, order.orderId.toString(), order.orderStatus)}
                  onDragEnd={handleDragEnd}
                >
                  <div className="flex items-center justify-between mb-2">
                    <span className="text-sm font-medium">ID: {order.orderId}</span>
                    <Badge className={`${getOrderStatusClass(order.orderStatus)}`}>
                      {order.orderStatus}
                    </Badge>
                  </div>
                  <div className="flex justify-between items-center text-sm text-gray-400">
                    <span>{order.createdAt ? new Date(order.createdAt).toLocaleDateString() : ''}</span>
                    <span className="font-medium text-primary-purple">{order.totalAmount} ₽</span>
                  </div>
                </div>
              ))
            ) : (
              <p className="text-sm text-gray-400 text-center py-3">Нет заказов</p>
            )}
          </div>
        </div>
      </CardContent>
    </Card>
  );
};

export default ClientPreviewCard;