import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../store/store";
import { fetchClientsWithOrdersAndTasks } from "../features/clients/clientSlice";
import Sidebar from "../components/StaticElements/Sidebar";
import TopBox from "../components/StaticElements/TopBox";
import OrderStatusColumn from "../components/kanban/OrderStatusColumn";
import { OrderStatus } from "../types/Order";
import LoadingSpinner from "../components/LoadingSpinner";

export const DashboardKanban = () => {
  const dispatch = useDispatch();
  const { clients, loading, error } = useSelector((state: RootState) => state.clients);
  const [isSidebarExpanded, setIsSidebarExpanded] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      await dispatch(fetchClientsWithOrdersAndTasks() as never);
    };
    fetchData();
  }, [dispatch]);

  const filterClientsByOrderStatus = (status: OrderStatus) => {
    if (!clients) return [];
    return clients.filter(client => {
      const hasMatchingOrder = client.orders?.some(order => {
        return order.orderStatus === status;
      });
      return hasMatchingOrder;
    });
  };

  return (
    <div className="w-screen h-screen bg-gradient-to-br from-indigo-950 via-purple-950 to-slate-900 text-white overflow-hidden">
      <Sidebar isExpanded={isSidebarExpanded} setIsExpanded={setIsSidebarExpanded} />
      <TopBox isExpanded={isSidebarExpanded} />

      <div className={`transition-all duration-300 mt-20 ${
        isSidebarExpanded ? "ml-[280px] w-[calc(100%-280px)]" : "ml-[100px] w-[calc(100%-100px)]"
      } h-[calc(100vh-80px)] overflow-y-auto`}>
        {loading ? (
          <div className="flex justify-center items-center h-full">
            <div className="relative">
              <div className="absolute inset-0 bg-primary-purple/20 blur-3xl rounded-full" />
              <div className="relative z-10 flex flex-col items-center">
                <h1 className="text-3xl font-bold text-primary-purple mb-2">Loading...</h1>
                <p className="text-gray-400 mb-6">Preparing Kanban board</p>
                <LoadingSpinner />
              </div>
            </div>
          </div>
        ) : error ? (
          <p className="text-red-500">{error}</p>
        ) : !clients || clients.length === 0 ? (
          <p className="text-gray-500">No data to display</p>
        ) : (
          <div className="flex gap-6 p-6 min-w-max">
            <OrderStatusColumn
              title="New orders"
              status={OrderStatus.New}
              clients={filterClientsByOrderStatus(OrderStatus.New)}
              columnColor="from-blue-900/40 to-blue-900/10"
            />
            <OrderStatusColumn
              title="Processing"
              status={OrderStatus.Processing}
              clients={filterClientsByOrderStatus(OrderStatus.Processing)}
              columnColor="from-yellow-900/40 to-yellow-900/10"
            />
            <OrderStatusColumn
              title="Completed"
              status={OrderStatus.Completed}
              clients={filterClientsByOrderStatus(OrderStatus.Completed)}
              columnColor="from-green-500/40 to-green-500/10"
            />
          </div>
        )}
      </div>
    </div>
  );
};

export default DashboardKanban;
