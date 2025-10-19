import React, { useState } from "react";
import api from "../api/api";
import { Button, TextField, Box, Typography } from "@mui/material";

export default function Register() {
  const [form, setForm] = useState({ username: "", password: "" });
  const [message, setMessage] = useState("");

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await api.post("/auth/register", form);
      setMessage("Registered successfully! You can now log in.");
    } catch (err) {
      console.log(err);
      setMessage(err.response?.data?.message || "Error registering user.");
    }
  };

  return (
    <Box sx={{ maxWidth: 400, mx: "auto", mt: 5 }}>
      <Typography variant="h5" mb={2}>Register</Typography>
      <form onSubmit={handleSubmit}>
        <TextField fullWidth label="Username" name="username" onChange={handleChange} margin="normal" />
        <TextField fullWidth type="password" label="Password" name="password" onChange={handleChange} margin="normal" />
        <Button variant="contained" fullWidth type="submit">Register</Button>
      </form>
      <Typography mt={2}>{message}</Typography>
    </Box>
  );
}
