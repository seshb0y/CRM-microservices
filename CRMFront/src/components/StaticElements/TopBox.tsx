import { Avatar, Box, IconButton, Menu, MenuItem, Typography } from "@mui/material";
import { Logout } from "@mui/icons-material";
import { useState } from "react";
import UserProfileModal from "../Modals/UserProfileModal";
import { useNavigate } from "react-router-dom";
import { useAppDispatch } from "../../hooks/useAppDispatch";
import { logoutUser } from "../../features/auth/authSlice";
import Lottie from "lottie-react";
import { useSelector } from "react-redux";
import { RootState } from "../../store/store";
import LogoAnimation from "../../assets/CRMLogoAnimation.json";

interface TopBoxProps {
  isExpanded: boolean;
}

const TopBox = ({ isExpanded }: TopBoxProps) => {
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const user = useSelector((state: RootState) => state.auth.user);

  const handleMenuOpen = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleLogOut = () => {
    dispatch(logoutUser());
    navigate("/");
  };

  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        padding: "5px 24px",
        background: "linear-gradient(to right, rgba(30, 27, 75, 0.9), rgba(88, 28, 135, 0.8))",
        backdropFilter: "blur(8px)",
        borderBottom: "1px solid rgba(139, 92, 246, 0.1)",
        position: "fixed",
        top: 0,
        left: 0,
        right: 0,
        transition: "all 0.3s ease",
        zIndex: 10,
        boxShadow: "0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)",
        paddingLeft: isExpanded ? "250px" : "84px",
      }}
    >
      {user && <UserProfileModal open={isProfileOpen} onClose={() => setIsProfileOpen(false)} user={user} />}

      <Typography 
        variant="h6" 
        sx={{ 
          display: "flex", 
          alignItems: "center", 
          color: "#fff",
          fontWeight: "bold",
          textShadow: "0 2px 4px rgba(0,0,0,0.1)",
        }}
      >
        <Lottie animationData={LogoAnimation} style={{ width: 80, height: 50, marginRight: 10 }} />
        CRMSolution
      </Typography>

      <Box 
        sx={{ 
          display: "flex", 
          alignItems: "center",
          marginLeft: "auto",
        }}
      >
        <Typography 
          sx={{ 
            marginRight: "10px", 
            color: "#fff",
            fontSize: "0.95rem",
            fontWeight: "500",
          }}
        >
          {user?.username || "User"}
        </Typography>
        <IconButton 
          onClick={handleMenuOpen}
          sx={{
            transition: "transform 0.2s ease",
            "&:hover": {
              transform: "scale(1.05)",
            }
          }}
        >
          <Avatar 
            sx={{ 
              bgcolor: "transparent",
              background: "linear-gradient(135deg, #8B5CF6 0%, #6D28D9 100%)",
              boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
            }}
          >
            {user?.username?.charAt(0).toUpperCase() || "U"}
          </Avatar>
        </IconButton>

        <Menu
          anchorEl={anchorEl}
          open={open}
          onClose={handleMenuClose}
          sx={{ 
            "& .MuiPaper-root": { 
              backgroundColor: "rgba(30, 27, 75, 0.95)",
              backdropFilter: "blur(8px)",
              color: "#fff",
              border: "1px solid rgba(139, 92, 246, 0.1)",
              boxShadow: "0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)",
            },
            "& .MuiMenuItem-root": {
              transition: "background-color 0.2s ease",
              "&:hover": {
                backgroundColor: "rgba(139, 92, 246, 0.1)",
              }
            }
          }}
        >
          <MenuItem
            onClick={() => {
              handleMenuClose();
              setIsProfileOpen(true);
            }}
          >
            Профиль
          </MenuItem>
          <MenuItem onClick={handleLogOut}>
            <Logout fontSize="small" sx={{ marginRight: "8px", color: "#8B5CF6" }} />
            Выйти
          </MenuItem>
        </Menu>
      </Box>
    </Box>
  );
};

export default TopBox;
