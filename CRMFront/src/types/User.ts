import { Order } from "./Order"
import { Task } from "./Task"

export interface User {
    userId: string;
    username: string;
    email: string;
    userRole: 0 | 1;
    role: 0 | 1;
    isEmailConfirmed: boolean;
    createdAt: Date;
    orders: Order[];
    tasks: Task[];
}

export const UserRole = {
    Admin: 0,
    Manager: 1
} as const;