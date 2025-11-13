export default function Home() {
  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center">
      <div className="text-center">
        <h1 className="text-4xl font-bold text-gray-800 mb-4">Welcome to ABC System</h1>
        <p className="text-gray-600 mb-8">You are successfully logged in!</p>
        <a 
          href="/login" 
          className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors duration-200"
        >
          Go to Login
        </a>
      </div>
    </div>
  );
}
