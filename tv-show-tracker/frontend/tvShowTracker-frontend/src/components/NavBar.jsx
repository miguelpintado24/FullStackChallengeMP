import React from "react";
import { AppBar, Toolbar, Button, Typography } from "@mui/material";
import { Link, useNavigate } from "react-router-dom";

export default function NavBar() {
  const navigate = useNavigate();
  const token = localStorage.getItem("token");

  const logout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <AppBar position="static" sx={{ mb: 2 }}>
      <Toolbar>
        <Typography variant="h6" sx={{ flexGrow: 1 }}>TV Show Tracker</Typography>
        {token && (
          <>
            <Button color="inherit" component={Link} to="/tvshows">Shows</Button>
            <Button color="inherit" component={Link} to="/favorites">Favorites</Button>
            <Button color="inherit" onClick={logout}>Logout</Button>
          </>
        )}
        {!token && (
          <>
            <Button color="inherit" component={Link} to="/login">Login</Button>
            <Button color="inherit" component={Link} to="/register">Register</Button>
          </>
        )}
      </Toolbar>
    </AppBar>
  );
}
