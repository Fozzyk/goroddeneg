// src/stores/auth.js
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/services/api'

export const useAuthStore = defineStore('auth', () => {
  const user  = ref(JSON.parse(localStorage.getItem('user') || 'null'))
  const token = ref(localStorage.getItem('token') || '')

  const isAuth  = computed(() => !!token.value)
  const isAdmin = computed(() => user.value?.role === 'Admin')

  async function login(email, password) {
    const { data } = await authApi.login({ email, password })
    _set(data)
    return data
  }

  async function register(payload) {
    const { data } = await authApi.register(payload)
    _set(data)
    return data
  }

  async function fetchMe() {
    try {
      const { data } = await authApi.me()
      user.value = data
      localStorage.setItem('user', JSON.stringify(data))
    } catch { logout() }
  }

  function logout() {
    token.value = ''; user.value = null
    localStorage.removeItem('token'); localStorage.removeItem('user')
  }

  function updateUser(patch) {
    if (user.value) {
      Object.assign(user.value, patch)
      localStorage.setItem('user', JSON.stringify(user.value))
    }
  }

  function _set(data) {
    token.value = data.accessToken; user.value = data.user
    localStorage.setItem('token', data.accessToken)
    localStorage.setItem('user', JSON.stringify(data.user))
  }

  return { user, token, isAuth, isAdmin, login, register, fetchMe, logout, updateUser }
})
