import { useState, useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import { RootState } from '../../store/store';
import { Task, TaskStatus } from '../../types/Task';

interface TaskSearchProps {
  onTaskSelect: (task: Task) => void;
}

export const TaskSearch = ({ onTaskSelect }: TaskSearchProps) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [isOpen, setIsOpen] = useState(false);
  const wrapperRef = useRef<HTMLDivElement>(null);
  const tasks = useSelector((state: RootState) => state.tasks.tasks);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (wrapperRef.current && !wrapperRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const filteredTasks = searchTerm.trim() === '' ? [] : tasks.filter(task => 
    (task.taskId?.toString() || '').includes(searchTerm.toLowerCase()) ||
    (task.title?.toLowerCase() || '').includes(searchTerm.toLowerCase()) ||
    (task.status?.toString() || '').includes(searchTerm.toLowerCase()) ||
    (task.username?.toLowerCase() || '').includes(searchTerm.toLowerCase())
  );

  const handleSelect = (task: Task) => {
    onTaskSelect(task);
    setSearchTerm('');
    setIsOpen(false);
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

  const getStatusColor = (status: number): string => {
    switch (status) {
      case TaskStatus.New:
        return 'text-blue-300';
      case TaskStatus.InProgress:
        return 'text-yellow-300';
      case TaskStatus.Completed:
        return 'text-green-300';
      default:
        return 'text-gray-300';
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
          placeholder="Поиск задач..."
          className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
        />
      </div>

      {isOpen && filteredTasks.length > 0 && (
        <div className="absolute z-50 w-full mt-2 bg-[#2a1042] rounded-lg shadow-xl max-h-60 overflow-y-auto">
          {filteredTasks.map((task) => (
            <div
              key={task.taskId}
              onClick={() => handleSelect(task)}
              className="px-4 py-2 hover:bg-[#3a1a5e] cursor-pointer transition-colors"
            >
              <div className="flex justify-between items-center">
                <div>
                  <div className="font-medium">#{task.taskId} {task.title}</div>
                  <div className="text-sm text-gray-400">{task.username}</div>
                </div>
                <div className="text-right">
                  <div className={`text-sm ${getStatusColor(task.status)}`}>
                    {getStatusText(task.status)}
                  </div>
                  <div className="text-sm text-gray-400">
                    {task.dueDate ? new Date(task.dueDate).toLocaleDateString() : '-'}
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}; 