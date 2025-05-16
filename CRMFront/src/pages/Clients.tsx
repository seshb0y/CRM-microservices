import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "../store/store";
import { useAppDispatch } from "../hooks/useAppDispatch";
import { fetchClients } from "../features/clients/clientSlice";
import Sidebar from "../components/StaticElements/Sidebar";
import TopBox from "../components/StaticElements/TopBox";
import LoadingScreen from "../components/LoadingScreen";
import ClientModal from "../components/Modals/ClientModal";
import ClientCreateModal from "../components/Modals/ClientCreateModal";
import { Client } from "../types/Client";
import { ClientSearch } from '../components/Search/ClientSearch';

const ITEMS_PER_PAGE = 15;
const MAX_VISIBLE_PAGES = 5;

export const ClientsPage = () => {
  const dispatch = useAppDispatch();
  const { clients, loading, error } = useSelector((state: RootState) => state.clients);
  const [isSidebarExpanded, setIsSidebarExpanded] = useState(false);
  const [selectedClient, setSelectedClient] = useState<Client | null>(null);
  const [isClientModalOpen, setIsClientModalOpen] = useState(false);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [sortClients, setSortClients] = useState<{ sortBy: string; descending: boolean }>({
    sortBy: "id",
    descending: false,
  });
  
  useEffect(() => {
    dispatch(fetchClients(sortClients));
  }, [dispatch, sortClients]);

  const openCreateClientModal = () => {
    setIsCreateModalOpen(true);
  };

  const openClientModal = (client: Client) => {
    setSelectedClient(client);
    setIsClientModalOpen(true);
  };

  const handleSort = (key: string) => {
    setSortClients((prev) => ({
      sortBy: key,
      descending: prev.sortBy === key ? !prev.descending : false,
    }));
    dispatch(fetchClients({ sortBy: key, descending: sortClients.sortBy === key ? !sortClients.descending : false }));
  };

  const totalPages = Math.ceil(clients.length / ITEMS_PER_PAGE);
  const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
  const endIndex = startIndex + ITEMS_PER_PAGE;
  const currentClients = clients.slice(startIndex, endIndex);

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  const getVisiblePages = () => {
    const pages: (number | string)[] = [];
    
    if (totalPages <= MAX_VISIBLE_PAGES) {
      return Array.from({ length: totalPages }, (_, i) => i + 1);
    }

    pages.push(1);

    let startPage = Math.max(2, currentPage - 1);
    let endPage = Math.min(totalPages - 1, currentPage + 1);

    if (currentPage <= 3) {
      endPage = Math.min(totalPages - 1, MAX_VISIBLE_PAGES - 1);
    } else if (currentPage >= totalPages - 2) {
      startPage = Math.max(2, totalPages - MAX_VISIBLE_PAGES + 2);
    }

    if (startPage > 2) {
      pages.push('...');
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    if (endPage < totalPages - 1) {
      pages.push('...');
    }

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
            <LoadingScreen title="Clients" subtitle="Loading clients data..." />
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
                Clients
              </h1>
              <div className="flex items-center gap-4">
                <div className="w-72">
                  <ClientSearch onClientSelect={openClientModal} />
                </div>
                <button 
                  className="bg-gradient-to-r from-purple-600 to-purple-800 hover:from-purple-700 hover:to-purple-900 px-4 py-2 rounded-lg text-white transition-all duration-300 hover:shadow-lg hover:shadow-purple-500/20"
                  onClick={openCreateClientModal}
                >
                  + Add Client
                </button>
              </div>
            </div>

            <div className="bg-gradient-to-br from-[rgba(30,27,75,0.95)] to-[rgba(88,28,135,0.9)] backdrop-blur-sm rounded-lg overflow-hidden shadow-[4px_0_6px_-1px_rgba(0,0,0,0.1),2px_0_4px_-1px_rgba(0,0,0,0.06)] border-r border-purple-500/10">
              <table className="w-full">
                <thead>
                  <tr>
                    {[
                      { key: "id", label: "ID", width: "auto" },
                      { key: "name", label: "Name", width: "auto" },
                      { key: "email", label: "Email", width: "auto" },
                      { key: "phone", label: "Phone", width: "auto" },
                      { key: "address", label: "Address", width: "auto" },
                      { key: "createdAt", label: "Created At", width: "auto" }
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
                            {sortClients.sortBy === key && (
                              sortClients.descending ? '↓' : '↑'
                            )}
                          </span>
                        </div>
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {loading ? (
                    <tr>
                      <td colSpan={6} className="h-[400px]">
                        <div className="flex justify-center items-center h-full">
                          <div className="relative">
                            <div className="w-12 h-12 border-4 border-purple-400/20 rounded-full animate-spin border-t-purple-400"></div>
                            <div className="mt-4 text-purple-300 text-sm">Загрузка данных...</div>
                          </div>
                        </div>
                      </td>
                    </tr>
                  ) : error ? (
                    <tr>
                      <td colSpan={6} className="h-[100px]">
                        <div className="flex justify-center items-center h-full">
                          <p className="text-red-400">{error}</p>
                        </div>
                      </td>
                    </tr>
                  ) : (
                    currentClients.map((client) => (
                      <tr 
                        key={client.id} 
                        className="border-b border-purple-500/10 hover:bg-[rgba(139,92,246,0.1)] transition-all duration-200 cursor-pointer"
                        onClick={() => openClientModal(client)}
                      >
                        <td className="py-2 px-4 text-white/90 tracking-wide font-inter">{client.id}</td>
                        <td className="py-2 px-4 text-white/90 tracking-wide font-inter">{client.name}</td>
                        <td className="py-2 px-4 text-white/90 tracking-wide font-inter">{client.email}</td>
                        <td className="py-2 px-4 text-white/90 tracking-wide font-inter">{client.phone}</td>
                        <td className="py-2 px-4 text-white/90 tracking-wide font-inter">{client.address}</td>
                        <td className="py-2 px-4 text-white/90 tracking-wide font-inter whitespace-nowrap">{new Date(client.createdAt).toLocaleDateString()}</td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>

              {!loading && !error && clients.length > 0 && (
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

      {isCreateModalOpen && <ClientCreateModal onClose={() => setIsCreateModalOpen(false)} />}
      {isClientModalOpen && selectedClient !== null && <ClientModal client={selectedClient} onClose={() => setIsClientModalOpen(false)} />}
    </div>
  );
};

export default ClientsPage;
