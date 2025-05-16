import { Client } from "./Client";
import { Task } from "./Task";
import { User } from "./User";

export enum OrderStatus {
    New = "New",
    Processing = "Processing",
    Completed = "Completed",
}

export interface Order {
    id?: number;
    orderId: string;
    numOrderId: number;
    totalAmount: number;
    createdAt?: Date;
    tasks?: Task[];
    status: OrderStatus;
    orderStatus: OrderStatus;
    users?: User[];
    client?: Client;
    callRecordingUrl?: string;
    username?: string;
    clientName?: string;
    budget?: number;
    description?: string;
}

export interface CreateOrder {
    totalAmount: number;
    clientEmail: string;
    userEmail: string;
    userId: number;
}

