import { Avatar, Box, Button, Modal, Typography, Divider, Chip } from "@mui/material";
import { User, UserRole } from "../../types/User";
import axiosInstance from "../../api/axiosInstance";
import { Email, Person, Verified, Warning } from "@mui/icons-material";
import { toast } from "react-toastify";

const UserProfileModal = ({ open, onClose, user }: { open: boolean; onClose: () => void; user: User }) => {
    const handleConfirmEmail = async () => {
        try {
            await axiosInstance.post("/account/email/confirm", { username: user.username });
            toast.success("Confirmation email has been sent successfully! Please check your inbox.", {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                theme: "dark",
            });
        } catch (error) {
            toast.error("Failed to send confirmation email. Please try again later.", {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                theme: "dark",
            });
            console.error("Error confirming email:", error);
        }
    };

    return (
        <Modal open={open} onClose={onClose} aria-labelledby="user-profile-modal">
            <Box
                sx={{
                    position: "absolute",
                    top: "50%",
                    left: "50%",
                    transform: "translate(-50%, -50%)",
                    width: 500,
                    bgcolor: "rgba(15, 13, 42, 0.95)",
                    color: "#fff",
                    boxShadow: "0 8px 32px 0 rgba(82, 0, 255, 0.2)",
                    p: 4,
                    borderRadius: 3,
                    border: "1px solid rgba(139, 92, 246, 0.1)",
                    backdropFilter: "blur(20px)",
                }}
            >
                <Box sx={{ display: "flex", flexDirection: "column", alignItems: "center", textAlign: "center" }}>
                    <Box
                        sx={{
                            position: "relative",
                            mb: 3,
                            "&::after": {
                                content: '""',
                                position: "absolute",
                                top: -2,
                                left: -2,
                                right: -2,
                                bottom: -2,
                                background: "linear-gradient(45deg, rgba(49, 46, 129, 1), rgba(139, 92, 246, 1))",
                                borderRadius: "50%",
                                zIndex: -1,
                            },
                        }}
                    >
                        <Avatar
                            sx={{
                                width: 100,
                                height: 100,
                                bgcolor: "rgba(15, 13, 42, 0.95)",
                                fontSize: "2.5rem",
                                border: "4px solid rgba(15, 13, 42, 0.95)",
                            }}
                        >
                            {user?.username?.charAt(0).toUpperCase()}
                        </Avatar>
                    </Box>

                    <Typography variant="h5" sx={{ fontWeight: "bold", mb: 1 }}>
                        {user.username || "Username"}
                    </Typography>

                    <Box sx={{ display: "flex", alignItems: "center", justifyContent: "center", gap: 1, mb: 2 }}>
                        <Email sx={{ fontSize: 20, opacity: 0.7 }} />
                        <Typography variant="body1" sx={{ opacity: 0.8 }}>
                            {user?.email || "example@email.com"}
                        </Typography>
                        {user.isEmailConfirmed ? (
                            <Chip
                                icon={<Verified />}
                                label="Verified"
                                size="small"
                                color="success"
                                sx={{ ml: 1 }}
                            />
                        ) : (
                            <Chip
                                icon={<Warning />}
                                label="Not Verified"
                                size="small"
                                color="warning"
                                sx={{ ml: 1 }}
                            />
                        )}
                    </Box>

                    <Box sx={{ display: "flex", alignItems: "center", justifyContent: "center", gap: 1, mb: 3 }}>
                        <Person sx={{ fontSize: 20, opacity: 0.7 }} />
                        <Typography variant="body1" sx={{ opacity: 0.8 }}>
                            {Object.keys(UserRole)[user.role] || "User"}
                        </Typography>
                    </Box>
                </Box>

                <Divider sx={{ bgcolor: "rgba(255, 255, 255, 0.1)", my: 2 }} />

                <Box sx={{ mt: 2, display: "flex", flexDirection: "column", gap: 2 }}>
                    {!user.isEmailConfirmed && (
                        <Button
                            variant="contained"
                            color="primary"
                            fullWidth
                            onClick={handleConfirmEmail}
                            startIcon={<Email />}
                            sx={{
                                background: "linear-gradient(45deg, rgba(49, 46, 129, 1) 30%, rgba(139, 92, 246, 1) 90%)",
                                "&:hover": {
                                    background: "linear-gradient(45deg, rgba(67, 56, 202, 1) 30%, rgba(124, 58, 237, 1) 90%)",
                                },
                            }}
                        >
                            Confirm Email
                        </Button>
                    )}

                    <Button
                        variant="outlined"
                        color="secondary"
                        fullWidth
                        onClick={onClose}
                        sx={{
                            borderColor: "rgba(139, 92, 246, 0.3)",
                            color: "rgba(255, 255, 255, 0.9)",
                            "&:hover": {
                                borderColor: "rgba(139, 92, 246, 0.5)",
                                backgroundColor: "rgba(139, 92, 246, 0.1)",
                            },
                        }}
                    >
                        Close
                    </Button>
                </Box>
            </Box>
        </Modal>
    );
};

export default UserProfileModal;
