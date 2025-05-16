import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "../ui/Table";
import { useSelector } from "react-redux";
import { RootState } from "../../store/store";
import { TaskStatus } from "../../types/Task";

interface TasksData {
  metric: string;
  value: number;
}

export const TasksStatusTable = () => {
  const tasksStatuses = useSelector((state: RootState) => state.dashboard.tasksStatuses);
  const tasksCount = useSelector((state: RootState) => state.dashboard.tasksCount);

  const statusCounts = {
    New: tasksStatuses.filter(status => status === TaskStatus.New).length,
    InProgress: tasksStatuses.filter(status => status === TaskStatus.InProgress).length,
    Completed: tasksStatuses.filter(status => status === TaskStatus.Completed).length,
  };

  const tasksData: TasksData[] = [
    { metric: "Total Tasks", value: tasksCount },
    { metric: "New Tasks", value: statusCounts.New },
    { metric: "In Progress", value: statusCounts.InProgress },
    { metric: "Completed", value: statusCounts.Completed },
  ];

  return (
    <div className="w-full">
      <Table>
        <TableHeader>
          <TableRow className="border-b border-purple-500/20">
            <TableHead className="text-purple-300 font-medium">Metric</TableHead>
            <TableHead className="text-center text-purple-300 font-medium">Value</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {tasksData.map((task) => (
            <TableRow 
              key={task.metric}
              className="border-b border-purple-500/10 hover:bg-purple-500/5 transition-colors"
            >
              <TableCell className="text-gray-300 text-center">{task.metric}</TableCell>
              <TableCell className="text-center font-medium text-white">{task.value}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
};

export default TasksStatusTable;
