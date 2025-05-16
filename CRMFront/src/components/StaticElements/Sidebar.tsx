import { Drawer, List, ListItemButton, ListItemIcon, ListItemText, Toolbar } from "@mui/material";
import { NavLink } from "react-router-dom";
import Lottie from "lottie-react";
import DashboardIcon from "@mui/icons-material/Dashboard";
import PeopleIcon from "@mui/icons-material/People";
import AssignmentIcon from "@mui/icons-material/Assignment";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import SettingsIcon from "@mui/icons-material/Settings";
import GroupIcon from "@mui/icons-material/Group";
import ViewKanbanIcon from "@mui/icons-material/ViewKanban";
import { useAppSelector } from "../../hooks/useAppSelector";

import DashboardAnimation from "../../assets/DashboardAnimation.json";
import ClientsAnimation from "../../assets/ClientsAnimation.json";
import OrdersAnimation from "../../assets/OrdersAnimation.json";
import TasksAnimation from "../../assets/TasksAnimation.json";
import UsersAnimation from "../../assets/UsersAnimation.json";
import KanbanAnimation from "../../assets/KanbanAnimation.json";
import SettingsAnimation from "../../assets/SettingsAnimation.json";
import { useState } from "react";

const menuItems = [
  { text: "Dashboard", icon: <DashboardIcon />, animation: DashboardAnimation, path: "/dashboard", roles: [0, 1] },
  { text: "Clients", icon: <PeopleIcon />, animation: ClientsAnimation, path: "/clients", roles: [0] },
  { text: "Orders", icon: <ShoppingCartIcon />, animation: OrdersAnimation, path: "/orders", roles: [0] },
  { text: "Tasks", icon: <AssignmentIcon />, animation: TasksAnimation, path: "/tasks", roles: [0] },
  { text: "Users", icon: <GroupIcon />, animation: UsersAnimation, path: "/users", roles: [0] },
  { text: "Kanban", icon: <ViewKanbanIcon />, animation: KanbanAnimation, path: "/kanban", roles: [0, 1] },
];

interface SidebarProps {
  isExpanded: boolean;
  setIsExpanded: (expanded: boolean) => void;
}

const Sidebar = ({ isExpanded, setIsExpanded }: SidebarProps) => {
  const [hoveredItem, setHoveredItem] = useState<string | null>(null);
  const user = useAppSelector(state => state.auth.user);
  
  const filteredMenuItems = menuItems.filter(item => 
    item.roles.includes(user?.role ?? 1)
  );

  return (
    <Drawer
      variant="permanent"
      onMouseEnter={() => setIsExpanded(true)}
      onMouseLeave={() => setIsExpanded(false)}
      sx={{
        width: isExpanded ? 240 : 60,
        flexShrink: 0,
        transition: "width 0.3s ease",
        "& .MuiDrawer-paper": {
          width: isExpanded ? 240 : 60,
          boxSizing: "border-box",
          background: "linear-gradient(180deg, rgba(30, 27, 75, 0.95) 0%, rgba(88, 28, 135, 0.9) 100%)",
          backdropFilter: "blur(8px)",
          color: "#fff",
          transition: "all 0.3s ease",
          borderRight: "1px solid rgba(139, 92, 246, 0.1)",
          boxShadow: "4px 0 6px -1px rgba(0, 0, 0, 0.1), 2px 0 4px -1px rgba(0, 0, 0, 0.06)",
        },
      }}
    >
      <Toolbar />
      <List>
        {filteredMenuItems.map(({ text, icon, animation, path }) => (
          <ListItemButton
            key={text}
            component={NavLink}
            to={path}
            onMouseEnter={() => setHoveredItem(text)}
            onMouseLeave={() => setHoveredItem(null)}
            sx={{
              color: "#fff",
              margin: "4px 8px",
              borderRadius: "8px",
              transition: "all 0.2s ease",
              "&.active": {
                background: "linear-gradient(135deg, rgba(139, 92, 246, 0.2), rgba(124, 58, 237, 0.1))",
                boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
                marginTop: "8px",
                marginBottom: "8px",
              },
              "&:hover": {
                background: "linear-gradient(135deg, rgba(139, 92, 246, 0.1), rgba(124, 58, 237, 0.05))",
                transform: "translateX(4px)",
              },
            }}
          >
            <ListItemIcon 
              sx={{ 
                color: "#fff", 
                minWidth: "40px",
                transition: "transform 0.2s ease",
                "&:hover": {
                  transform: "scale(1.1)",
                },
              }}
            >
              {hoveredItem === text ? (
                <Lottie animationData={animation} style={{ width: 60, height: 40, marginLeft: -20 }} />
              ) : (
                icon
              )}
            </ListItemIcon>
            {isExpanded && (
              <ListItemText 
                primary={text} 
                sx={{
                  "& .MuiListItemText-primary": {
                    fontWeight: "500",
                    fontSize: "0.95rem",
                    transition: "color 0.2s ease",
                  },
                }}
              />
            )}
          </ListItemButton>
        ))}
      </List>
    </Drawer>
  );
};

export default Sidebar;
