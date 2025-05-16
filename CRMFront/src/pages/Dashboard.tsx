import { useSelector } from "react-redux";
import { RootState } from "../store/store";
import { StatsCard } from "../components/Dashboard/StatsCard";
import { NewDealChart } from "../components/Dashboard/NewDealChart";
import { useAppDispatch } from "../hooks/useAppDispatch";
import { useEffect, useState } from "react";
import { getDashboardData } from "../features/dashboard/dashboardSlice";
import { TasksStatusTable } from "../components/Dashboard/TasksInfoTable";
import Sidebar from "../components/StaticElements/Sidebar";
import LoadingScreen from "../components/LoadingScreen";
import { fetchGetAllOrders } from "../features/orders/orderSlice";
import { checkAuth } from "../features/auth/authSlice";
import TopBox from "../components/StaticElements/TopBox";

export const Dashboard = () => {
  const { clientsAmount: contacts, ordersCount, ordersTotalAmount, loading } = useSelector(
    (state: RootState) => state.dashboard
  );


  const dispatch = useAppDispatch();
  const [isSidebarExpanded, setIsSidebarExpanded] = useState(false);
  const [showCumulative, setShowCumulative] = useState(true);

  useEffect(() => {
    dispatch(getDashboardData());
    dispatch(checkAuth());
    dispatch(fetchGetAllOrders({ sortBy: "createdAt", descending: true }));
  }, [dispatch]);

  return (
    <div className="w-max h-screen bg-gradient-to-br from-indigo-950 via-purple-950 to-slate-900 text-white overflow-hidden">
      <Sidebar isExpanded={isSidebarExpanded} setIsExpanded={setIsSidebarExpanded} />
      <TopBox isExpanded={isSidebarExpanded} />

      <div
        className={`transition-all duration-300 p-6 mt-20  ${
          isSidebarExpanded ? "ml-[200px]" : "ml-[0px]"
        } w-screen`}
      >
        {loading ? (
          <LoadingScreen 
            title="Loading Dashboard" 
            subtitle="Please, wait while data is loading..." 
          />
        ) : (
          <div className="px-16 pr-[calc(1rem*4-70px)]">
            <div className="grid grid-cols-2 gap-8">
              <div>
                <div className="grid grid-cols-2 gap-6 mb-8">
                  <StatsCard title="Clients" value={contacts} change={-12} />
                  <StatsCard title="Orders" value={ordersCount} change={-4} />
                </div>
                <div className="bg-gradient-to-br from-indigo-900 to-purple-900 backdrop-blur-sm rounded-lg p-6 min-h-[400px] shadow-xl">
                  <NewDealChart 
                    showCumulative={showCumulative}
                    onToggleView={() => setShowCumulative(!showCumulative)}
                  />
                </div>
              </div>

              <div>
                <StatsCard
                  title="Orders Amount" 
                  value={ordersTotalAmount} 
                  unit="$" 
                  change={-37} 
                />
                <div className="mt-8 bg-gradient-to-br from-indigo-900 to-purple-900 backdrop-blur-sm rounded-lg p-6 shadow-xl">
                  <div className="flex items-center justify-between mb-6">
                    <h2 className="text-xl font-medium text-white">Tasks Info Table</h2>
                  </div>
                  <div className="bg-[#0f0d2a]/80 rounded-lg p-4">
                    <TasksStatusTable />
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Dashboard;
