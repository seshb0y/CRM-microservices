import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "../store/store";
import { useAppDispatch } from "../hooks/useAppDispatch";
import { fetchGetAllTasks } from "../features/tasks/tasksSlice";
import Sidebar from "../components/StaticElements/Sidebar";
import TopBox from "../components/StaticElements/TopBox";
import LoadingScreen from "../components/LoadingScreen";
import { useNavigate } from "react-router-dom";
import { Task, TaskStatus } from "../types/Task";
import { TaskSearch } from "../components/Search/TaskSearch";

const ITEMS_PER_PAGE = 15;
const MAX_VISIBLE_PAGES = 5; // Максимальное количество видимых кнопок страниц

export const TasksPage = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { tasks, loading, error } = useSelector((state: RootState) => state.tasks);
  const [isSidebarExpanded, setIsSidebarExpanded] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [sortTask, setSortTask] = useState<{ sortBy: string; descending: boolean }>({
    sortBy: "id",
    descending: false,
  });

  useEffect(() => {
    dispatch(fetchGetAllTasks(sortTask));
  }, [dispatch, sortTask]);

  console.log(tasks)
  const handleSort = (key: string) => {
    setSortTask((prev) => ({
      sortBy: key,
      descending: prev.sortBy === key ? !prev.descending : false,
    }));
    dispatch(fetchGetAllTasks({ sortBy: key, descending: sortTask.sortBy === key ? !sortTask.descending : false }));
  };

  const handleTaskSelect = (task: Task) => {
    navigate(`/orders/${task.orderId}`);
  };

  const getStatusColor = (status: number): string => {
    switch (status) {
      case TaskStatus.New:
        return 'bg-blue-500/20 text-blue-300';
      case TaskStatus.InProgress:
        return 'bg-yellow-500/20 text-yellow-300';
      case TaskStatus.Completed:
        return 'bg-green-500/20 text-green-300';
      default:
        return 'bg-red-500/20 text-red-300';
    }
  };

  const getStatusText = (status: number): string => {
    switch (status) {
      case TaskStatus.New:
        return "New";
      case TaskStatus.InProgress:
        return "InProgress";
      case TaskStatus.Completed:
        return "Completed";
      default:
        return "Unknown";
    }
  };

  const totalPages = Math.ceil(tasks.length / ITEMS_PER_PAGE);
  const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
  const endIndex = startIndex + ITEMS_PER_PAGE;
  const currentTasks = tasks.slice(startIndex, endIndex);

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  const getVisiblePages = () => {
    const pages: (number | string)[] = [];
    
    if (totalPages <= MAX_VISIBLE_PAGES) {
      // Если страниц меньше или равно MAX_VISIBLE_PAGES, показываем все
      return Array.from({ length: totalPages }, (_, i) => i + 1);
    }

    // Всегда показываем первую страницу
    pages.push(1);

    // Вычисляем начальную и конечную страницу для отображения
    let startPage = Math.max(2, currentPage - 1);
    let endPage = Math.min(totalPages - 1, currentPage + 1);

    // Корректируем диапазон, если мы близко к началу или концу
    if (currentPage <= 3) {
      endPage = Math.min(totalPages - 1, MAX_VISIBLE_PAGES - 1);
    } else if (currentPage >= totalPages - 2) {
      startPage = Math.max(2, totalPages - MAX_VISIBLE_PAGES + 2);
    }

    // Добавляем многоточие после первой страницы, если есть пропуск
    if (startPage > 2) {
      pages.push('...');
    }

    // Добавляем страницы в диапазоне
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    // Добавляем многоточие перед последней страницей, если есть пропуск
    if (endPage < totalPages - 1) {
      pages.push('...');
    }

    // Всегда показываем последнюю страницу
    if (totalPages > 1) {
      pages.push(totalPages);
    }

    return pages;
  };

  return (
    <div className="w-screen h-screen bg-gradient-to-br from-indigo-950 via-purple-950 to-slate-900 text-white overflow-hidden">
      <Sidebar isExpanded={isSidebarExpanded} setIsExpanded={setIsSidebarExpanded} />
      <TopBox isExpanded={isSidebarExpanded} />

      <div
        className={`transition-all duration-300 mt-20 ${
          isSidebarExpanded ? "ml-[280px] w-[calc(100%-280px)]" : "ml-[100px] w-[calc(100%-100px)]"
        } h-[calc(100vh-80px)] overflow-y-auto`}
      >
        {loading ? (
          <div className="flex justify-center items-center h-full">
            <LoadingScreen title="Tasks" subtitle="Loading tasks data..." />
          </div>
        ) : error ? (
          <div className="px-6">
            <div className="bg-gradient-to-br from-red-900/50 to-purple-900/50 backdrop-blur-sm rounded-lg p-4 shadow-xl">
              <p className="text-red-400">{error}</p>
            </div>
          </div>
        ) : (
          <div className="px-6">
            <div className="flex justify-between items-center mb-4">
              <h1 className="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-purple-200 to-purple-400">
                Tasks
              </h1>
              <div className="flex items-center gap-4">
                <div className="w-72">
                  <TaskSearch onTaskSelect={handleTaskSelect} />
                </div>
              </div>
            </div>

            <div className="bg-gradient-to-br from-[rgba(30,27,75,0.95)] to-[rgba(88,28,135,0.9)] backdrop-blur-sm rounded-lg overflow-hidden shadow-[4px_0_6px_-1px_rgba(0,0,0,0.1),2px_0_4px_-1px_rgba(0,0,0,0.06)] border-r border-purple-500/10">
              <table className="w-full">
                <thead>
                  <tr>
                    {[
                      { key: "taskId", label: "ID", width: "100px" },
                      { key: "title", label: "Title", width: "300px" },
                      { key: "status", label: "Status", width: "150px" },
                      { key: "dueDate", label: "Due Date", width: "150px" },
                      { key: "username", label: "Responsible", width: "200px" }
                    ].map(({ key, label, width }) => (
                      <th 
                        key={key}
                        onClick={() => handleSort(key)}
                        className="py-3 px-4 text-left text-white font-medium tracking-wide text-[0.95rem] cursor-pointer transition-all group sticky top-0 bg-[rgba(30,27,75,0.98)] border-b border-purple-500/20"
                        style={{ width }}
                      >
                        <div className="flex items-center gap-2">
                          {label}
                          <span className="text-purple-400/70 group-hover:text-purple-300 transition-colors">
                            {sortTask.sortBy === key && (
                              sortTask.descending ? '↓' : '↑'
                            )}
                          </span>
                        </div>
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {currentTasks.map((task) => (
                    <tr 
                      key={task.taskId} 
                      className="border-b border-purple-500/10 hover:bg-[rgba(139,92,246,0.1)] transition-all duration-200 cursor-pointer"
                      onClick={() => handleTaskSelect(task)}
                    >
                      <td className="py-2 px-4 text-white/90 tracking-wide font-inter">#{task.taskId}</td>
                      <td className="py-2 px-4 text-white/90 tracking-wide font-inter">{task.title}</td>
                      <td className="py-2 px-4 text-white/90 tracking-wide font-inter">
                        <span className={`px-2 py-1 rounded-full text-xs ${getStatusColor(task.status)}`}>
                          {getStatusText(task.status)}
                        </span>
                      </td>
                      <td className="py-2 px-4 text-white/90 tracking-wide font-inter whitespace-nowrap">
                        {new Date(task.dueDate || '').toLocaleDateString()}
                      </td>
                      <td className="py-2 px-4 text-white/90 tracking-wide font-inter">{task.username}</td>
                    </tr>
                  ))}
                </tbody>
              </table>

              {/* Пагинация */}
              {!loading && !error && tasks.length > 0 && (
                <div className="flex justify-center items-center gap-2 py-4 bg-[rgba(30,27,75,0.98)] border-t border-purple-500/20">
                  <button
                    onClick={() => handlePageChange(currentPage - 1)}
                    disabled={currentPage === 1}
                    className="px-3 py-1 rounded-lg bg-purple-600/50 hover:bg-purple-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
                  >
                    ←
                  </button>
                  
                  {getVisiblePages().map((page, index) => (
                    page === '...' ? (
                      <span 
                        key={`ellipsis-${index}`}
                        className="px-3 py-1 text-purple-400"
                      >
                        ...
                      </span>
                    ) : (
                      <button
                        key={page}
                        onClick={() => handlePageChange(page as number)}
                        className={`px-3 py-1 rounded-lg transition-all ${
                          currentPage === page
                            ? 'bg-purple-600 text-white'
                            : 'bg-purple-600/50 hover:bg-purple-600'
                        }`}
                      >
                        {page}
                      </button>
                    )
                  ))}
                  
                  <button
                    onClick={() => handlePageChange(currentPage + 1)}
                    disabled={currentPage === totalPages}
                    className="px-3 py-1 rounded-lg bg-purple-600/50 hover:bg-purple-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
                  >
                    →
                  </button>
                </div>
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default TasksPage;
