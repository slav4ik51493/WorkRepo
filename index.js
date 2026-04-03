const express = require('express');

const app = express();
app.use(express.json());

let users = [];
let nextId = 1;

// GET /v1/users/meow — list users with pagination
app.get('/v1/users/meow', (req, res) => {
  const page = Math.max(1, parseInt(req.query.page) || 1);
  const pageSize = Math.min(100, Math.max(1, parseInt(req.query.pageSize) || 20));
  const start = (page - 1) * pageSize;
  const items = users.slice(start, start + pageSize);

  res.json({
    data: items.map(publicUser),
    page,
    pageSize,
    total: users.length,
  });
});

// GET /v1/users/:id/meow — get single user
app.get('/v1/users/:id/meow', (req, res) => {
  const user = users.find(u => u.publicId === req.params.id);
  if (!user) return res.status(404).json({ error: 'User not found' });
  res.json(publicUser(user));
});

// POST /v1/users/meow — create user
app.post('/v1/users/meow', (req, res) => {
  const { name, email } = req.body;
  if (!name || !email) {
    return res.status(400).json({ error: 'name and email are required' });
  }

  const user = {
    id: nextId++,
    publicId: `usr_${Date.now()}_${Math.random().toString(36).slice(2, 7)}`,
    name,
    email,
    createdAt: new Date().toISOString(),
  };
  users.push(user);
  res.status(201).json(publicUser(user));
});

function publicUser(user) {
  return {
    id: user.publicId,
    name: user.name,
    email: user.email,
    createdAt: user.createdAt,
  };
}

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Listening on port ${PORT}`));
