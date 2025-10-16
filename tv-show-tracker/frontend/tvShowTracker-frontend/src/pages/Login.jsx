import React, { useState } from "react";
import api from "../api/api";
import { Button, TextField, Box, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";

export default function Login() {
  const [form, setForm] = useState({ username: "", password: "" });
  const [message, setMessage] = useState("");
  const navigate = useNavigate();

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const res = await api.post("/auth/login", form);
      localStorage.setItem("token", res.data.token);
      navigate("/");
    } catch (err) {
      setMessage(err.response?.data?.message || "Invalid credentials.");
    }
  };

  return (
    <Box sx={{ maxWidth: 400, mx: "auto", mt: 5 }}>
      <Typography variant="h5" mb={2}>Login</Typography>
      <form onSubmit={handleSubmit}>
        <TextField fullWidth label="Username" name="username" onChange={handleChange} margin="normal" />
        <TextField fullWidth type="password" label="Password" name="password" onChange={handleChange} margin="normal" />
        <Button variant="contained" fullWidth type="submit">Login</Button>
      </form>
      <Typography mt={2}>{message}</Typography>
    </Box>
  );
}
