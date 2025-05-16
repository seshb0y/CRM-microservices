import { LineChart, Line, XAxis, YAxis, Tooltip, ResponsiveContainer } from "recharts";
import { useSelector } from "react-redux";
import { RootState } from "../../store/store";
import { format } from "date-fns";

interface NewDealChartProps {
  showCumulative: boolean;
  onToggleView: () => void;
}

export const NewDealChart: React.FC<NewDealChartProps> = ({ showCumulative, onToggleView }) => {
  const orders = useSelector((state: RootState) => {
    return state.orders.orders;
  });

  if (!orders) {
    return null;
  }

  if (orders.length === 0) {
    return <div className="text-gray-400 text-center py-4">Нет данных для отображения</div>;
  }


  const sortedOrders = [...orders].sort(
    (a, b) => {
      const dateA = a.createdAt ? new Date(a.createdAt).getTime() : 0;
      const dateB = b.createdAt ? new Date(b.createdAt).getTime() : 0;
      return dateA - dateB;
    }
  );


  let cumulativeAmount = 0;
  const formattedData = sortedOrders.map((order, index) => {
    const amount = Number(order.totalAmount) || 0;
    cumulativeAmount += amount;
    return {
      name: `Deal ${index + 1}`,
      amount: showCumulative ? cumulativeAmount : amount,
      date: order.createdAt ? format(new Date(order.createdAt), "MMM d") : "-",
      fullDate: order.createdAt ? format(new Date(order.createdAt), "yyyy-MM-dd") : "-",
    };
  });


  return (
    <div className="h-[400px] w-full">
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-xl font-medium text-white">
          {showCumulative ? "Cumulative Deal Amount" : "Individual Deal Amount"}
        </h2>
        <button 
          onClick={onToggleView}
          className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-lg text-sm transition-colors"
        >
          {showCumulative ? "Show Individual" : "Show Cumulative"}
        </button>
      </div>
      <div className="bg-[#0f0d2a]/80 backdrop-blur-sm rounded-lg p-4 h-[300px]">
        <ResponsiveContainer width="100%" height="100%">
          <LineChart 
            data={formattedData}
            margin={{ top: 10, right: 30, left: 10, bottom: 10 }}
          >
            <XAxis 
              dataKey="date" 
              stroke="#9CA3AF"
              fontSize={12}
              tickLine={false}
              axisLine={false}
            />
            <YAxis 
              stroke="#9CA3AF"
              fontSize={12}
              tickLine={false}
              axisLine={false}
              tickFormatter={(value) => `$${value}`}
            />
            <Tooltip
              content={({ active, payload }) => {
                if (active && payload && payload.length) {
                  const data = payload[0].payload;
                  return (
                    <div className="bg-indigo-900/90 backdrop-blur-sm p-3 rounded-lg border border-purple-500/20 shadow-xl">
                      <p className="text-white">{data.fullDate}</p>
                      <p className="text-white font-bold">${data.amount}</p>
                      <p className="text-gray-400">{data.name}</p>
                    </div>
                  );
                }
                return null;
              }}
            />
            <Line 
              type={showCumulative ? "monotone" : "linear"}
              dataKey="amount" 
              stroke="#A855F7"
              strokeWidth={2}
              dot={{
                stroke: "#A855F7",
                strokeWidth: 2,
                r: 4,
                fill: "#1e1b4b"
              }}
            />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
};

export default NewDealChart;


