import React, { useEffect, useState } from "react";
import api from "../api/api";
import { Card, CardContent, Typography, Grid, Button } from "@mui/material";

export default function Favorites() {
  const [favorites, setFavorites] = useState([]);

  useEffect(() => {
    api.get("/tvshows/favorites")
      .then((res) => setFavorites(res.data))
      .catch(() => setFavorites([]));
  }, []);

  const removeFavorite = async (id) => {
    await api.delete(`/tvshows/${id}/favorite`);
    setFavorites(favorites.filter((f) => f.id !== id));
  };

  return (
    <Grid container spacing={2} sx={{ p: 2 }}>
      {favorites.map((fav) => (
        <Grid item xs={12} sm={6} md={4} key={fav.id}>
          <Card sx={{ p: 2 }}>
            <CardContent>
              <Typography variant="h6">{fav.title}</Typography>
              <Typography color="text.secondary">{fav.genre}</Typography>
              <Button onClick={() => removeFavorite(fav.id)}>Remove ❌</Button>
            </CardContent>
          </Card>
        </Grid>
      ))}
      {favorites.length === 0 && (
        <Typography variant="body1" sx={{ mt: 4, mx: "auto" }}>
          You have no favorite shows yet.
        </Typography>
      )}
    </Grid>
  );
}
