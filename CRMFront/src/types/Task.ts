import { Order } from "./Order"
import { User } from "./User"

export enum TaskStatus {
    New = 0,
    InProgress = 1,
    Completed = 2
}

export interface Task {
    taskId: string;
    id?: number;
    title: string;
    description?: string;
    status: TaskStatus;
    dueDate?: Date;
    order?: Order;
    userTasks?: { user: User }[];
    username?: string;
    orderId?: string;
}

export interface CreateTask {
    title: string;
    description: string;
    endDate: Date;
    userName: string;
    orderId: number;
}