import LoadingScreen from "./LoadingScreen";

const LoadingPage = () => {
  return (
    <div className="w-screen h-screen bg-gradient-to-br from-indigo-950 via-purple-950 to-slate-900 text-white overflow-hidden">
      <LoadingScreen title="Загрузка" subtitle="Загрузка страницы..." />
    </div>
  );
};

export default LoadingPage; 