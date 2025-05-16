import { useState, useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import { RootState } from '../../store/store';
import { User } from '../../types/User';

interface UserSearchProps {
  onUserSelect: (user: User) => void;
}

export const UserSearch = ({ onUserSelect }: UserSearchProps) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [isOpen, setIsOpen] = useState(false);
  const wrapperRef = useRef<HTMLDivElement>(null);
  const users = useSelector((state: RootState) => state.users.users);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (wrapperRef.current && !wrapperRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const filteredUsers = searchTerm.trim() === '' ? [] : users.filter(user => 
    (user.userId?.toString() || '').includes(searchTerm.toLowerCase()) ||
    (user.username?.toLowerCase() || '').includes(searchTerm.toLowerCase()) ||
    (user.email?.toLowerCase() || '').includes(searchTerm.toLowerCase())
  );

  const handleSelect = (user: User) => {
    onUserSelect(user);
    setSearchTerm('');
    setIsOpen(false);
  };

  const getRoleText = (role: number): string => {
    return role === 0 ? "Администратор" : "Пользователь";
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
          placeholder="Поиск пользователей..."
          className="w-full px-4 py-2 bg-[#2a1042] text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500/50"
        />
      </div>

      {isOpen && filteredUsers.length > 0 && (
        <div className="absolute z-50 w-full mt-2 bg-[#2a1042] rounded-lg shadow-xl max-h-60 overflow-y-auto">
          {filteredUsers.map((user) => (
            <div
              key={user.userId}
              onClick={() => handleSelect(user)}
              className="px-4 py-2 hover:bg-[#3a1a5e] cursor-pointer transition-colors"
            >
              <div className="flex justify-between items-center">
                <div>
                  <div className="font-medium">{user.username}</div>
                  <div className="text-sm text-gray-400">{user.email}</div>
                </div>
                <div className="text-right">
                  <div className={`text-sm ${user.userRole === 1 ? 'text-purple-400' : 'text-blue-400'}`}>
                    {getRoleText(user.userRole)}
                  </div>
                  <div className="text-sm text-gray-400">
                    {new Date(user.createdAt).toLocaleDateString()}
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