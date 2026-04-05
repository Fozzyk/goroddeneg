// src/services/api.js
import axios from 'axios'

const http = axios.create({ baseURL: '/api', headers: { 'Content-Type': 'application/json' } })

http.interceptors.request.use(cfg => {
  const t = localStorage.getItem('token')
  if (t) cfg.headers.Authorization = `Bearer ${t}`
  return cfg
})

http.interceptors.response.use(r => r, err => {
  if (err.response?.status === 401) {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    window.location.href = '/login'
  }
  return Promise.reject(err)
})

export const authApi = {
  register: d => http.post('/auth/register', d),
  login:    d => http.post('/auth/login', d),
  me:       () => http.get('/auth/me'),
  changePassword: d => http.post('/auth/change-password', d),
}

export const projectsApi = {
  getAll:       p  => http.get('/projects', { params: p }),
  getById:      id => http.get(`/projects/${id}`),
  getMy:        () => http.get('/projects/my'),
  create:       d  => http.post('/projects', d),
  uploadCover:  (id, f) => { const fd = new FormData(); fd.append('file', f); return http.post(`/projects/${id}/cover`, fd, { headers: { 'Content-Type': 'multipart/form-data' } }) },
  selectPkg:    (id, d) => http.post(`/projects/${id}/package`, d),
  pledge:       (id, d) => http.post(`/projects/${id}/pledge`, d),
}

export const categoriesApi = { getAll: () => http.get('/categories') }

export const usersApi = {
  getProfile:    () => http.get('/users/profile'),
  updateProfile: d  => http.put('/users/profile', d),
  uploadAvatar:  f  => { const fd = new FormData(); fd.append('file', f); return http.post('/users/avatar', fd, { headers: { 'Content-Type': 'multipart/form-data' } }) },
  getTransactions: p => http.get('/users/transactions', { params: p }),
  topUp:         d  => http.post('/users/topup', d),
  getPledges:    () => http.get('/users/pledges'),
  getLegal:      () => http.get('/users/legal'),
  saveLegal:     d  => http.post('/users/legal', d),
}

export const adminApi = {
  getStats:       () => http.get('/admin/stats'),
  getProjects:    p  => http.get('/admin/projects', { params: p }),
  updateStatus:   (id, d) => http.put(`/admin/projects/${id}/status`, d),
  getUsers:       p  => http.get('/admin/users', { params: p }),
  getTransactions: p => http.get('/admin/transactions', { params: p }),
}

export default http
