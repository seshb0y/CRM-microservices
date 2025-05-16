interface ButtonProps {
    stroke: string;
  }
  
  const Button = ({ stroke }: ButtonProps) => {
    return (
      <div>
        <button className="w-full bg-purple-600 hover:bg-purple-700 text-white font-semibold py-3 rounded-md transition-all">
          {stroke}
        </button>
      </div>
    );
  };
  
  export default Button;
  