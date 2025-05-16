import { motion } from "framer-motion";
import { Card, CardContent, CardHeader, CardTitle } from "../ui/card";
import { useNavigate } from "react-router-dom";

interface StatsCardProps {
  title: string;
  value: number | string;
  change: number;
  unit?: string;
}

export const StatsCard: React.FC<StatsCardProps> = ({ title, value, unit }) => {
  const navigate = useNavigate();

  const getRoute = (title: string) => {
    switch (title.toLowerCase()) {
      case "clients":
        return "/clients";
      case "orders":
        return "/orders";
      default:
        return "/dashboard";
    }
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
      className="w-full"
    >
      <Card 
        className="bg-gradient-to-br from-indigo-900 to-purple-900 backdrop-blur-sm border-none shadow-xl rounded-lg transition-all duration-300 hover:shadow-purple-500/10 hover:scale-[1.02] cursor-pointer h-[160px] group"
        onClick={() => navigate(getRoute(title))}
      >
        <CardHeader className="p-6">
          <CardTitle className="text-lg font-medium text-gray-300 group-hover:text-white transition-colors">{title}</CardTitle>
        </CardHeader>
        <CardContent className="p-6 pt-0">
          <div className={`flex ${title === "Orders Amount" ? "justify-start" : "justify-end"}`}>
            <div className="text-7xl font-bold text-white">
              {unit}{value}
            </div>
          </div>
        </CardContent>
      </Card>
    </motion.div>
  );
};

export default StatsCard;
