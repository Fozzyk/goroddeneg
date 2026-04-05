<template>
  <div class="auth-page">
    <div class="auth-box">
      <div class="auth-logo">
        <router-link to="/" class="logo-link">
          <div class="logo-icon">Г</div>
          <span>Город <strong>Денег</strong></span>
        </router-link>
      </div>
      <h1 class="auth-title">Добро пожаловать</h1>
      <p class="auth-sub">Войдите в свой аккаунт</p>

      <div v-if="error" class="alert alert-danger" style="margin-top:16px">{{ error }}</div>

      <form @submit.prevent="submit" class="auth-form">
        <div class="form-group">
          <label class="form-label">Email</label>
          <input v-model="email" type="email" class="form-control" placeholder="you@example.com" required autocomplete="email" />
        </div>
        <div class="form-group">
          <label class="form-label">Пароль</label>
          <div class="pw-wrap">
            <input v-model="password" :type="showPw?'text':'password'" class="form-control" placeholder="••••••••" required autocomplete="current-password" />
            <button type="button" class="pw-eye" @click="showPw=!showPw">{{ showPw?'🙈':'👁' }}</button>
          </div>
        </div>
        <div class="demo-box">
          <div class="demo-title">Демо-аккаунты для теста:</div>
          <div class="demo-row"><b>Пользователь:</b> aibek@goroddeneg.kz / User@123456!</div>
          <div class="demo-row"><b>Администратор:</b> admin@goroddeneg.kz / Admin@123456!</div>
        </div>
        <button type="submit" class="btn btn-primary btn-full" :disabled="loading">
          <span v-if="loading" class="spinner" style="width:16px;height:16px" />
          <span v-else>Войти</span>
        </button>
      </form>
      <div class="auth-footer">Нет аккаунта? <router-link to="/register">Зарегистрируйтесь</router-link></div>
    </div>
  </div>
</template>
<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
const auth = useAuthStore(); const router = useRouter()
const email = ref(''); const password = ref(''); const loading = ref(false); const error = ref(''); const showPw = ref(false)
async function submit() {
  error.value = ''; loading.value = true
  try { await auth.login(email.value, password.value); await router.push(auth.isAdmin?'/admin':'/cabinet') }
  catch(e) { error.value = e.response?.data?.message || 'Неверный email или пароль' }
  loading.value = false
}
</script>
<style scoped>
.auth-page { min-height: 100vh; display: flex; align-items: center; justify-content: center; padding: 40px 16px; background: var(--bg-soft); }
.auth-box { width: 100%; max-width: 420px; background: var(--bg); border: 1px solid var(--border); border-radius: var(--r-xl); padding: 40px; box-shadow: var(--shadow-md); }
.auth-logo { text-align: center; margin-bottom: 28px; }
.logo-link { display: inline-flex; align-items: center; gap: 10px; text-decoration: none; }
.logo-icon { width: 36px; height: 36px; border-radius: 10px; background: var(--blue); color: #fff; font-family: 'Bricolage Grotesque', sans-serif; font-size: 19px; font-weight: 800; display: flex; align-items: center; justify-content: center; }
.logo-link span { font-family: 'Bricolage Grotesque', sans-serif; font-size: 17px; font-weight: 600; color: var(--text); }
.logo-link strong { color: var(--blue); }
.auth-title { font-size: 1.6rem; font-weight: 800; text-align: center; margin-bottom: 6px; }
.auth-sub { text-align: center; color: var(--text-muted); font-size: 14px; }
.auth-form { margin-top: 24px; }
.pw-wrap { position: relative; }
.pw-wrap .form-control { padding-right: 44px; }
.pw-eye { position: absolute; right: 12px; top: 50%; transform: translateY(-50%); background: none; border: none; cursor: pointer; font-size: 16px; }
.demo-box { background: var(--blue-light); border: 1px solid var(--blue-mid); border-radius: var(--r-sm); padding: 12px 14px; font-size: 12px; color: var(--text-mid); line-height: 1.7; margin-bottom: 20px; }
.demo-title { font-weight: 600; color: var(--blue); margin-bottom: 4px; }
.auth-footer { text-align: center; margin-top: 20px; font-size: 14px; color: var(--text-muted); }
.auth-footer a { color: var(--blue); font-weight: 600; }
</style>
