import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import api from "../api/api";
import { Box, Typography, Card, CardContent, Grid, Divider, Button } from "@mui/material";

export default function ShowDetails() {
  const { id } = useParams();
  const [show, setShow] = useState(null);
  const [episodes, setEpisodes] = useState([]);
  const [actors, setActors] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        // Fetch show info
        const showRes = await api.get(`/tvshows/${id}`);
        setShow(showRes.data);

        // Fetch episodes
        const episodeRes = await api.get(`/tvshows/${id}/episodes`);
        setEpisodes(episodeRes.data);

        // Fetch actors
        const actorRes = await api.get(`/tvshows/${id}/actors`);
        setActors(actorRes.data);
      } catch (err) {
        console.error("Error loading show details:", err);
      }
    };
    fetchData();
  }, [id]);

  if (!show) return <Typography sx={{ mt: 4, textAlign: "center" }}>Loading...</Typography>;

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>{show.title}</Typography>
      <Typography variant="subtitle1" color="text.secondary">{show.genre}</Typography>
      <Typography variant="body1" sx={{ my: 2 }}>{show.description || "No description available."}</Typography>
      <Divider sx={{ my: 3 }} />

      <Typography variant="h6" gutterBottom>🎭 Actors</Typography>
      <Grid container spacing={2}>
        {actors.length > 0 ? (
          actors.map((actor) => (
            <Grid item xs={12} sm={6} md={4} key={actor.id}>
              <Card>
                <CardContent>
                  <Typography variant="h6">{actor.name}</Typography>
                  <Typography color="text.secondary">{actor.role}</Typography>
                </CardContent>
              </Card>
            </Grid>
          ))
        ) : (
          <Typography sx={{ pl: 2 }}>No actors found.</Typography>
        )}
      </Grid>

      <Divider sx={{ my: 3 }} />

      <Typography variant="h6" gutterBottom>📅 Episodes</Typography>
      <Grid container spacing={2}>
        {episodes.length > 0 ? (
          episodes.map((ep) => (
            <Grid item xs={12} sm={6} md={4} key={ep.id}>
              <Card>
                <CardContent>
                  <Typography variant="h6">{ep.title}</Typography>
                  <Typography color="text.secondary">Season {ep.season} — Ep {ep.episodeNumber}</Typography>
                  <Typography variant="body2">
                    Released: {new Date(ep.releaseDate).toLocaleDateString()}
                  </Typography>
                </CardContent>
              </Card>
            </Grid>
          ))
        ) : (
          <Typography sx={{ pl: 2 }}>No episodes found.</Typography>
        )}
      </Grid>

      <Divider sx={{ my: 3 }} />

      <Button
        variant="contained"
        onClick={() => api.post(`/tvshows/${id}/favorite`).then(() => alert("Added to favorites!"))}
      >
        Add to Favorites
      </Button>
    </Box>
  );
}
