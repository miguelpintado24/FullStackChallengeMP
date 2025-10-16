import React, { useEffect, useState } from "react";
import api from "../api/api";
import { Card, CardContent, Typography, Grid, Button } from "@mui/material";
import { useNavigate } from "react-router-dom";

export default function TvShows() {
  const [shows, setShows] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    api.get("/tvshows?page=1&pageSize=10").then((res) => setShows(res.data.shows));
  }, []);

  const addFavorite = async (id) => {
    await api.post(`/tvshows/${id}/favorite`);
    alert("Added to favorites!");
  };

  return (
    <Grid container spacing={2} sx={{ p: 2 }}>
      {shows.map((show) => (
        <Grid item xs={12} sm={6} md={4} key={show.id}>
          <Card sx={{ p: 2 }}>
            <CardContent>
              <Typography variant="h6">{show.title}</Typography>
              <Typography color="text.secondary">{show.genre}</Typography>
              <Button size="small" onClick={() => navigate(`/shows/${show.id}`)}>Details</Button>
              <Button size="small" onClick={() => addFavorite(show.id)}>Favorite</Button>
            </CardContent>
          </Card>
        </Grid>
      ))}
    </Grid>
  );
}
