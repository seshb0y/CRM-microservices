import * as React from "react";
import { cn } from "../../lib/utils"; // Если используешь ShadCN

interface TableProps {
  children: React.ReactNode;
  className?: string;
}

export const Table = ({ children, className }: TableProps) => {
  return <table className={cn("w-full border-collapse", className)}>{children}</table>;
};

export const TableBody = ({ children, className }: TableProps) => {
  return <tbody className={className}>{children}</tbody>;
};

export const TableCell = ({ children, className }: TableProps) => {
  return <td className={cn("border px-4 py-2", className)}>{children}</td>;
};

export const TableHead = ({ children, className }: TableProps) => {
  return <th className={cn("border px-4 py-2 bg-gray-800 text-white", className)}>{children}</th>;
};

export const TableHeader = ({ children, className }: TableProps) => {
  return <thead className={className}>{children}</thead>;
};

export const TableRow = ({ children, className }: TableProps) => {
  return <tr className={className}>{children}</tr>;
};
