import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { AppDispatch } from "../../store/store";
import { useParams } from "react-router-dom";
import axiosInstance from "../../api/axiosInstance";
import { createTask, deleteTask } from "../../features/tasks/tasksSlice";
import Sidebar from "../StaticElements/Sidebar";
import TopBox from "../StaticElements/TopBox";
import { Order, OrderStatus } from "../../types/Order";
import { TaskStatus, Task } from "../../types/Task";
import { fetchChangeClientData } from "../../features/clients/clientSlice";
import OrderEditModal from "../Modals/OrderEditModal";
import { fetchUsers } from "../../features/user/userSlice";
import LoadingScreen from "../LoadingScreen";
import { toast } from 'react-toastify';

interface EditingTask {
  id: number;
  tittle: string;
  description: string;
  status: TaskStatus;
}

const OrderDetailsPage = () => {
  const { orderId } = useParams();
  const dispatch = useDispatch<AppDispatch>();
  const [order, setOrder] = useState<Order>();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>("");
  const [isSidebarExpanded, setIsSidebarExpanded] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [taskTitle, setTaskTitle] = useState("");
  const [taskDescription, setTaskDescription] = useState("");
  const [taskDueDate, setTaskDueDate] = useState("");
  const [deletingTaskIds, setDeletingTaskIds] = useState<number[]>([]);
  const [isEditTaskModalOpen, setIsEditTaskModalOpen] = useState(false);
  const [editingTask, setEditingTask] = useState<EditingTask | null>(null);
  const [editingTaskDescription, setEditingTaskDescription] = useState("");
  const [editingTaskStatus, setEditingTaskStatus] = useState<TaskStatus>(TaskStatus.New);
  const [updatingTaskId, setUpdatingTaskId] = useState<number | null>(null);
  const [isEditingClient, setIsEditingClient] = useState(false);
  const [editedClientData, setEditedClientData] = useState({
    name: "",
    email: "",
    phone: "",
    address: ""
  });
  const [isOrderEditModalOpen, setIsOrderEditModalOpen] = useState(false);
  const [shouldRefresh, setShouldRefresh] = useState(false);

  useEffect(() => {
    dispatch(fetchUsers({}));
  }, [dispatch]);

  const fetchOrderDetails = async () => {
    if (!orderId) return;
    
    try {
      const response = await axiosInstance.get(`/orders/${orderId}`);
      console.log("🔍 Received order:", response.data);
      setOrder(response.data);
    } catch (err) {
      setError("Order loading error.");
      console.error("Error loading order:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!orderId) {
      setError("Incorrect order ID.");
      setLoading(false);
      return;
    }

    fetchOrderDetails();
  }, [orderId]);

  useEffect(() => {
    const handleResponsibleUpdated = () => {
      setShouldRefresh(true);
    };

    const handleOrderUpdated = () => {
      setShouldRefresh(true);
    };

    const handleTaskCreated = () => {
      setShouldRefresh(true);
    };

    const handleTaskDeleted = () => {
      setShouldRefresh(true);
    };

    window.addEventListener('responsibleUpdated', handleResponsibleUpdated);
    window.addEventListener('orderUpdated', handleOrderUpdated);
    window.addEventListener('taskCreated', handleTaskCreated);
    window.addEventListener('taskDeleted', handleTaskDeleted);

    return () => {
      window.removeEventListener('responsibleUpdated', handleResponsibleUpdated);
      window.removeEventListener('orderUpdated', handleOrderUpdated);
      window.removeEventListener('taskCreated', handleTaskCreated);
      window.removeEventListener('taskDeleted', handleTaskDeleted);
    };
  }, []);

  useEffect(() => {
    if (shouldRefresh) {
      fetchOrderDetails();
      setShouldRefresh(false);
    }
  }, [shouldRefresh]);

  useEffect(() => {
    if (order?.client) {
      setEditedClientData({
        name: order.client.name,
        email: order.client.email,
        phone: order.client.phone,
        address: order.client.address
      });
    }
  }, [order?.client]);

  const handleCallClient = async () => {
    if (!order?.client?.phone) return;
  
    try {
      const response = await axiosInstance.post("twilio/calls", { to: order.client.phone });
      const callSid = response.data.callSid;
  
      console.log("Call initiated, CallSid:", callSid);
  
      setTimeout(async () => {
        try {
          console.log("start", callSid)
          const recordingRes = await axiosInstance.get(`twilio/recordings/${String(callSid)}`);
          const mediaUrl = recordingRes.data.recordingUrl;
  
          console.log("Recording URL received:", mediaUrl);
          
          await axiosInstance.post("twilio/recordings", {
            orderId: order.id,
            callSid: callSid,
          });
  
          setOrder(prevOrder => {
            if (!prevOrder) return prevOrder;
            return {
              ...prevOrder,
              callRecordingUrl: mediaUrl
            };
          });
  
        } catch (err) {
          console.error("Error getting call recording:", err);
        }
      }, 30000);
  
    } catch (err) {
      console.error("Error making call:", err);
    }
  };

  const handleCreateTask = async () => {
    if (!taskTitle.trim() || !taskDescription.trim() || !taskDueDate || !order?.id) {
      toast.error('Please fill in all required fields');
      return;
    }

    if (!order.users || order.users.length === 0 || !order.users[0].username) {
      toast.error('No responsible user assigned to this order');
      return;
    }

    const taskData = {
      title: taskTitle,
      description: taskDescription,
      endDate: new Date(taskDueDate),
      userName: order.users[0].username,
      orderId: order.id
    };

    try {
      await dispatch(createTask(taskData));
      setIsModalOpen(false);
      setTaskTitle("");
      setTaskDescription("");
      setTaskDueDate("");
      toast.success('Task created successfully');
    } catch (error) {
      toast.error('Failed to create task');
      console.error("Error creating task:", error);
    }
  };

  const handleDeleteTask = async (taskId: number) => {
    if (!order?.tasks) return;
    
    try {
      setDeletingTaskIds(prev => [...prev, taskId]);
      await dispatch(deleteTask(taskId));
      setOrder({
        ...order,
        tasks: order.tasks.filter(task => task.id === taskId)
      });
    } catch (error) {
      console.error("Ошибка при удалении задачи:", error);
    } finally {
      setDeletingTaskIds(prev => prev.filter(id => id !== taskId));
    }
  };

  const handleEditTask = async () => {
    if (!editingTask || !order?.tasks) return;
    
    try {
      setUpdatingTaskId(editingTask.id);
      
      const request = {
        taskId: editingTask.id,
        status: getTaskStatusString(editingTaskStatus),
        description: editingTaskDescription
      };

      await axiosInstance.put("/tasks/", request);

      setOrder({
        ...order,
        tasks: order.tasks.map(task =>
          task.id === editingTask.id
            ? { ...task, description: editingTaskDescription, status: editingTaskStatus }
            : task
        )
      });

      setIsEditTaskModalOpen(false);
      setEditingTask(null);
    } catch (err) {
      console.error("Error updating task:", err);
    } finally {
      setUpdatingTaskId(null);
    }
  };

  const openEditTaskModal = (task: EditingTask) => {
    setEditingTask(task);
    setEditingTaskDescription(task?.description || "");
    setEditingTaskStatus(task?.status || TaskStatus.New);
    setIsEditTaskModalOpen(true);
  };

  const handleClientDataChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setEditedClientData({
      ...editedClientData,
      [e.target.name]: e.target.value
    });
  };

  const formatPhoneNumber = (phone: string): string => {
    return phone.replace(/\D/g, '');
  };

  const formatPhoneNumberForDisplay = (phone: string): string => {
    const cleaned = phone.replace(/\D/g, '');
    
    if (cleaned.length < 3) return phone;
    
    // Определяем код страны (первые 1-3 цифры)
    const countryCode = cleaned.slice(0, 3);
    const localNumber = cleaned.slice(3);
    
    return `+${countryCode} ${localNumber}`;
  };

  const handleSaveClientData = async () => {
    if (!order?.client) return;

    try {
      const formattedPhone = formatPhoneNumber(editedClientData.phone);
      
      const result = await dispatch(fetchChangeClientData({
        name: editedClientData.name,
        newEmail: editedClientData.email,
        oldEmail: order.client.email,
        phone: formattedPhone,
        address: editedClientData.address
      })).unwrap();

      if (result) {
        setOrder({
          ...order,
          client: {
            ...order.client,
            name: editedClientData.name,
            email: editedClientData.email,
            phone: editedClientData.phone,
            address: editedClientData.address
          }
        });
        setIsEditingClient(false);
      }
    } catch {
      console.error("Ошибка при обновлении данных клиента");
    }
  };

  const handleOrderUpdate = (updatedOrder: Order) => {
    setOrder(updatedOrder);
    setIsOrderEditModalOpen(false);
  };

  const getStatusTextFromNumber = (status: number | string): string => {
    
    if (typeof status === 'string' && Object.values(OrderStatus).includes(status as OrderStatus)) {
      return status;
    }

    const numericStatus = typeof status === 'string' ? parseInt(status, 10) : status;
    
    switch (numericStatus) {
      case 0:
        return OrderStatus.New;
      case 1:
        return OrderStatus.Processing;
      case 2:
        return OrderStatus.Completed;
      default:
        return "Unknown";
    }
  };

  const getTaskStatusString = (status: number): string => {
    switch (status) {
      case TaskStatus.New:
        return "New";
      case TaskStatus.InProgress:
        return "In Progress";
      case TaskStatus.Completed:
        return "Completed";
      default:
        return "Undefined";
    }
  };

  const getCallRecordingDate = (url: string): Date => {
    if (typeof url !== 'string') return new Date(0);
    const file = url.split('/').pop()?.replace('.mp3', '');
    if (!file) return new Date(0);
    const parts = file.split('_');
    if (parts.length < 2) return new Date(0);
    const dateStr = parts[1];
    const date = new Date(dateStr);
    return isNaN(date.getTime()) ? new Date(0) : date;
  };

  const getCombinedItems = (order: Order) => {
    const items: { type: 'task' | 'call', date?: Date, task?: Task, url?: string }[] = [];
    if (order.tasks) {
      for (const task of order.tasks) {
        if (task.dueDate) {
          items.push({ type: 'task', date: new Date(task.dueDate), task });
        }
      }
    }
    if (Array.isArray(order.callRecordingUrl)) {
      for (const url of order.callRecordingUrl) {
        items.push({ type: 'call', url });
      }
    } else if (typeof order.callRecordingUrl === 'string') {
      items.push({ type: 'call', url: order.callRecordingUrl });
    }
    // Сортируем только задачи по дате, звонки идут после задач
    return [
      ...items.filter(i => i.type === 'task').sort((a, b) => (b.date?.getTime() || 0) - (a.date?.getTime() || 0)),
      ...items.filter(i => i.type === 'call')
    ];
  };

  if (loading) return <LoadingScreen title="Loading..." />;
  if (error) return <p className="text-red-500">{error}</p>;

  if (!order) {
    return <p className="text-red-500">Order data is missing</p>;
  }

  return (
    <div className="w-screen h-screen bg-gradient-to-br from-indigo-950 via-purple-950 to-slate-900 text-white overflow-hidden">
      <Sidebar isExpanded={isSidebarExpanded} setIsExpanded={setIsSidebarExpanded} />
      <TopBox isExpanded={isSidebarExpanded} />
      <div className={`transition-all duration-300 p-6 mt-20 ${
        isSidebarExpanded ? "ml-[280px] w-[calc(100%-280px)]" : "ml-[100px] w-[calc(100%-100px)]"
      }`}>
        <div className="flex gap-6">
          <div className="min-w-[400px] max-w-[400px] bg-[#1a0b2e] p-6 rounded-lg shadow-md">
            <h2 className="text-lg font-semibold mb-4">
              Deal #{order.id}
              <div className="flex justify-between items-center">
                <div>
                  <p className="text-white font-medium">Budget: {order.totalAmount.toLocaleString('en-US')} $</p>
                  <p className="text-white font-medium">Status: {getStatusTextFromNumber(order.status)}</p>
                  <p className="text-white font-medium">Responsible: {order.users?.[0]?.username || 'Not assigned'}</p>
                </div>
                <button
                  onClick={() => setIsOrderEditModalOpen(true)}
                  className="bg-primary-purple px-3 py-1 rounded text-sm"
                >
                  Edit
                </button>
              </div>
            </h2>
            <div className="text-white">
              {!order.client ? (
                <div className="p-4 bg-[#2a1042]/50 rounded-lg text-center">
                  <p className="text-gray-400">No client information available</p>
                </div>
              ) : isEditingClient ? (
                <div className="space-y-3">
                  {[
                    { label: "Client Name", name: "name" },
                    { label: "Email", name: "email" },
                    { label: "Phone", name: "phone" },
                    { label: "Address", name: "address" }
                  ].map((field) => (
                    <div key={field.name} className="flex flex-col">
                      <label className="text-sm text-white font-medium mb-1">{field.label}:</label>
                      <input
                        type="text"
                        name={field.name}
                        value={editedClientData[field.name as keyof typeof editedClientData]}
                        onChange={handleClientDataChange}
                        className="bg-[#2a1042] text-white px-2 py-1 rounded w-full"
                      />
                    </div>
                  ))}
                  <div className="flex gap-2 mt-4">
                    <button
                      onClick={handleSaveClientData}
                      className="bg-primary-purple px-4 py-2 rounded text-white flex-1"
                    >
                      Save
                    </button>
                    <button
                      onClick={() => {
                        setIsEditingClient(false);
                        if (order.client) {
                          setEditedClientData({
                            name: order.client.name,
                            email: order.client.email,
                            phone: order.client.phone,
                            address: order.client.address
                          });
                        }
                      }}
                      className="bg-gray-600 px-4 py-2 rounded text-white flex-1"
                    >
                      Cancel
                    </button>
                  </div>
                </div>
              ) : (
                <>
                  <div className="flex justify-between items-center mb-2">
                    <p><span className="text-gray-400  font-medium">Client:</span> {order.client.name}</p>
                    <button
                      onClick={() => setIsEditingClient(true)}
                      className="bg-primary-purple px-3 py-1 rounded text-sm"
                    >
                      Edit
                    </button>
                  </div>
                  <p><span className="text-gray-400 font-medium">Email:</span> {order.client.email}</p>
                  <p><span className="text-gray-400 font-medium">Phone:</span> {formatPhoneNumberForDisplay(order.client.phone)}</p>
                  <p><span className="text-gray-400 font-medium">Address:</span> {order.client.address}</p>
                </>
              )}
            </div>
            {order.client && (
              <button 
                onClick={handleCallClient} 
                className="bg-primary-purple px-4 py-2 rounded text-white mt-4">
                Call Client
              </button>
            )}
          </div>

          <div className="flex-1">
            <div className="bg-[#2a1042] p-6 rounded-lg shadow-md h-[calc(100vh-140px)] flex flex-col">
              <h2 className="text-lg font-semibold mb-4">Tasks & Calls</h2>
              {order.tasks && order.tasks.length > 0 || order.callRecordingUrl ? (
                <div className="space-y-4 overflow-y-auto flex-1 pr-2">
                  {getCombinedItems(order).map((item, idx) =>
                    item.type === 'task' && item.task ? (
                      <div key={`task-${item.task.id}`} className="bg-[#3a1a5e] p-4 rounded-md shadow-md">
                        <div className="flex justify-between items-start">
                          <div>
                            <h3 className="text-lg font-semibold">{item.task.title}</h3>
                            <p>{item.task.description}</p>
                            <p className="text-sm text-gray-400">Status: {getTaskStatusString(item.task.status)}</p>
                            <p className="text-sm text-gray-400">
                              Deadline: {item.task.dueDate ? new Date(item.task.dueDate).toLocaleDateString('en-US') : 'Not set'}
                            </p>
                          </div>
                          <div className="flex gap-2">
                            <button
                              onClick={() => item.task?.id && openEditTaskModal({
                                id: item.task?.id,
                                tittle: item.task?.title,
                                description: item.task?.description || '',
                                status: item.task?.status
                              })}
                              className={`${
                                updatingTaskId === item.task?.id
                                  ? "bg-purple-700"
                                  : "bg-primary-purple hover:bg-purple-700"
                              } text-white p-1.5 rounded-md text-sm transition-colors flex items-center justify-center w-8 h-8`}
                              disabled={updatingTaskId === item.task?.id}
                              title="Edit task"
                            >
                              {updatingTaskId === item.task?.id ? (
                                <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                              ) : (
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                                  <path d="M13.586 3.586a2 2 0 112.828 2.828l-.793.793-2.828-2.828.793-.793zM11.379 5.793L3 14.172V17h2.828l8.38-8.379-2.83-2.828z" />
                                </svg>
                              )}
                            </button>
                            <button
                              onClick={() => item.task?.id && handleDeleteTask(item.task.id)}
                              className={`${
                                deletingTaskIds.includes(item.task?.id || 0)
                                  ? "bg-red-700"
                                  : "bg-red-600 hover:bg-red-700"
                              } text-white p-1.5 rounded-md text-sm transition-colors flex items-center justify-center w-8 h-8`}
                              disabled={deletingTaskIds.includes(item.task?.id || 0)}
                              title="Delete task"
                            >
                              {deletingTaskIds.includes(item.task?.id || 0) ? (
                                <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                              ) : (
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                                  <path fillRule="evenodd" d="M9 2a1 1 0 00-.894.553L7.382 4H4a1 1 0 000 2v10a2 2 0 002 2h8a2 2 0 002-2V6a1 1 0 100-2h-3.382l-.724-1.447A1 1 0 0011 2H9zM7 8a1 1 0 012 0v6a1 1 0 11-2 0V8zm5-1a1 1 0 00-1 1v6a1 1 0 102 0V8a1 1 0 00-1-1z" clipRule="evenodd" />
                                </svg>
                              )}
                            </button>
                          </div>
                        </div>
                      </div>
                    ) : (
                      <div key={`call-${idx}`} className="bg-[#3a1a5e] p-4 rounded-md shadow-md">
                        <div className="flex items-center justify-between">
                          <div>
                            <p className="text-sm text-gray-400">Запись звонка</p>
                          </div>
                          <audio controls className="w-64">
                            <source src={item.url} type="audio/mpeg" />
                            Ваш браузер не поддерживает аудио элемент.
                          </audio>
                        </div>
                      </div>
                    )
                  )}
                </div>
              ) : (
                <div className="flex-1">
                  <p className="text-gray-400">No tasks or call recordings</p>
                </div>
              )}
              <div className="flex items-center justify-between mt-4 pt-4 border-t border-purple-500/20">
                <button
                  className="px-3 py-1.5 bg-primary-purple text-white text-sm rounded-md hover:bg-purple-700 transition flex items-center gap-2"
                  onClick={() => setIsModalOpen(true)}
                >
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                    <path fillRule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clipRule="evenodd" />
                  </svg>
                  Add Task
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      {isOrderEditModalOpen && order && (
        <OrderEditModal
          order={order}
          onClose={() => setIsOrderEditModalOpen(false)}
          onUpdate={handleOrderUpdate}
        />
      )}

      {isModalOpen && (
        <div className="fixed inset-0 flex justify-center items-center bg-black/50 backdrop-blur-sm z-50">
          <div className="bg-gradient-to-br from-[rgba(30,27,75,0.95)] to-[rgba(88,28,135,0.9)] p-8 rounded-lg w-[500px] max-h-[80vh] overflow-auto shadow-xl border border-purple-500/20">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-purple-200 to-purple-400">
                Create Task
              </h2>
              <button
                onClick={() => setIsModalOpen(false)}
                className="text-gray-400 hover:text-white transition-colors"
              >
                ✕
              </button>
            </div>

            <div className="space-y-4">
              <div>
                <label className="block text-sm text-gray-300 mb-1">Task Title</label>
                <input
                  type="text"
                  placeholder="Enter task title"
                  value={taskTitle}
                  onChange={(e) => setTaskTitle(e.target.value)}
                  className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
                />
              </div>

              <div>
                <label className="block text-sm text-gray-300 mb-1">Task Description</label>
                <textarea
                  placeholder="Enter task description"
                  value={taskDescription}
                  onChange={(e) => setTaskDescription(e.target.value)}
                  className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50 min-h-[100px]"
                />
              </div>

              <div>
                <label className="block text-sm text-gray-300 mb-1">Deadline</label>
                <input
                  type="date"
                  value={taskDueDate}
                  onChange={(e) => setTaskDueDate(e.target.value)}
                  className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
                />
              </div>

              <div className="flex gap-3 mt-6">
                <button
                  onClick={handleCreateTask}
                  className="flex-1 bg-gradient-to-r from-purple-600 to-purple-800 hover:from-purple-700 hover:to-purple-900 px-4 py-2 rounded-lg text-white transition-all duration-300 hover:shadow-lg hover:shadow-purple-500/20"
                >
                  Add
                </button>
                <button
                  onClick={() => setIsModalOpen(false)}
                  className="flex-1 bg-gradient-to-r from-gray-600 to-gray-800 hover:from-gray-700 hover:to-gray-900 px-4 py-2 rounded-lg text-white transition-all duration-300"
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {isEditTaskModalOpen && editingTask && (
        <div className="fixed inset-0 flex justify-center items-center bg-black/50 backdrop-blur-sm z-50">
          <div className="bg-gradient-to-br from-[rgba(30,27,75,0.95)] to-[rgba(88,28,135,0.9)] p-8 rounded-lg w-[500px] max-h-[80vh] overflow-auto shadow-xl border border-purple-500/20">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-purple-200 to-purple-400">
                Edit Task
              </h2>
              <button
                onClick={() => setIsEditTaskModalOpen(false)}
                className="text-gray-400 hover:text-white transition-colors"
              >
                ✕
              </button>
            </div>

            <div className="space-y-4">
              <div>
                <label className="block text-sm text-gray-300 mb-1">Title</label>
                <p className="text-white font-medium">{editingTask.tittle}</p>
              </div>

              <div>
                <label className="block text-sm text-gray-300 mb-1">Description</label>
                <textarea
                  value={editingTaskDescription}
                  onChange={(e) => setEditingTaskDescription(e.target.value)}
                  className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50 min-h-[100px]"
                  rows={3}
                />
              </div>

              <div>
                <label className="block text-sm text-gray-300 mb-1">Status</label>
                <select
                  value={editingTaskStatus}
                  onChange={(e) => setEditingTaskStatus(Number(e.target.value) as TaskStatus)}
                  className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
                >
                  <option value={TaskStatus.New}>New</option>
                  <option value={TaskStatus.InProgress}>In Progress</option>
                  <option value={TaskStatus.Completed}>Completed</option>
                </select>
              </div>

              <div className="flex gap-3 mt-6">
                <button
                  onClick={handleEditTask}
                  className="flex-1 bg-gradient-to-r from-purple-600 to-purple-800 hover:from-purple-700 hover:to-purple-900 px-4 py-2 rounded-lg text-white transition-all duration-300 hover:shadow-lg hover:shadow-purple-500/20"
                >
                  Save
                </button>
                <button
                  onClick={() => setIsEditTaskModalOpen(false)}
                  className="flex-1 bg-gradient-to-r from-gray-600 to-gray-800 hover:from-gray-700 hover:to-gray-900 px-4 py-2 rounded-lg text-white transition-all duration-300"
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default OrderDetailsPage;
