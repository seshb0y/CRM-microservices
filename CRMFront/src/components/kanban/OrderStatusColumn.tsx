import { useState } from "react";
import ClientPreviewCard from "../Card/ClientPreviewCard";
import { Client } from "../../types/Client";
import { OrderStatus } from "../../types/Order";
import { useDispatch, useSelector } from "react-redux";
import { fetchChangeOrderData } from "../../features/orders/orderSlice";
import { fetchClientsWithOrdersAndTasks } from "../../features/clients/clientSlice";
import { AppDispatch, RootState } from "../../store/store";
import LoadingSpinner from "../LoadingSpinner";

interface Props {
  title: string;
  status: OrderStatus;
  clients: Client[];
  columnColor: string;
}

const OrderStatusColumn = ({ title, status, clients, columnColor }: Props) => {
  const dispatch = useDispatch<AppDispatch>();
  const allClients = useSelector((state: RootState) => state.clients.clients);
  const [isUpdating, setIsUpdating] = useState(false);

  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    const column = e.currentTarget;
    column.style.opacity = "0.8";
  };

  const handleDragLeave = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    const column = e.currentTarget;
    column.style.opacity = "1";
  };

  const handleDrop = async (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    const column = e.currentTarget;
    column.style.opacity = "1";

    const orderId = e.dataTransfer.getData("orderId");
    const currentStatus = e.dataTransfer.getData("currentStatus");

    if (currentStatus !== status) {
      try {
        setIsUpdating(true);
        const currentOrder = allClients
          .flatMap(client => client.orders || [])
          .find(order => order.orderId.toString() === orderId);

        if (currentOrder) {
          const statusNumber = status === OrderStatus.New ? 0 : 
                             status === OrderStatus.Processing ? 1 : 2;
          
          await dispatch(fetchChangeOrderData({
            orderId: Number(orderId),
            status: statusNumber,
            totalAmount: currentOrder.totalAmount
          }));
          
          await dispatch(fetchClientsWithOrdersAndTasks() as never);
        }
      } catch (error) {
        console.error("Error updating order status:", error);
      } finally {
        setIsUpdating(false);
      }
    }
  };

  const filteredClients = clients
    .map(client => ({
      ...client,
      orders: client.orders?.filter(order => {
        return order.orderStatus === status;
      }) || []
    }))
    .filter(client => client.orders.length > 0);

  return (
    <div 
      className={`w-[500px] min-w-[350px] ml-20 bg-gradient-to-b ${columnColor} p-6 rounded-xl border border-gray-800/30 shadow-lg transition-all duration-300 ease-in-out hover:border-gray-700/50 relative`}
      onDragOver={handleDragOver}
      onDragLeave={handleDragLeave}
      onDrop={handleDrop}
    >
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-white">{title}</h2>
        <span className="px-3 py-1 text-sm bg-gray-800/50 rounded-full text-gray-300 backdrop-blur-sm">
          {filteredClients.reduce((acc, client) => acc + client.orders.length, 0)}
        </span>
      </div>
      {isUpdating && (
        <div className="absolute inset-0 bg-black/20 backdrop-blur-sm rounded-xl flex items-center justify-center">
          <LoadingSpinner />
        </div>
      )}
      <div className="space-y-4 overflow-y-auto max-h-[calc(100vh-250px)] pr-2 custom-scrollbar">
        {filteredClients.length > 0 ? (
          filteredClients.map(client => <ClientPreviewCard key={client.id} client={client} />)
        ) : (
          <div className="text-center py-8 px-4 bg-gray-800/30 rounded-lg border border-gray-700/30 backdrop-blur-sm">
            <p className="text-gray-400">No orders</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default OrderStatusColumn;
  