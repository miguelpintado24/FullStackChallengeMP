import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import Login from "./pages/Login";
import Register from "./pages/Register";
import TvShows from "./pages/TvShows";
import ShowDetails from "./pages/ShowDetails";
import Favorites from "./pages/Favorites";
import NavBar from "./components/NavBar";

function App() {
  const token = localStorage.getItem("token");

  return (
    <Router>
      <NavBar />
      <Routes>
        <Route path="/" element={token ? <TvShows /> : <Navigate to="/login" />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/shows/:id" element={<ShowDetails />} />
        <Route path="/favorites" element={<Favorites />} />
      </Routes>
    </Router>
  );
}

export default App;