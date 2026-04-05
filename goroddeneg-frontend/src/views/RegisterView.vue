<template>
  <div class="auth-page">
    <div class="auth-box">
      <div class="auth-logo">
        <router-link to="/" class="logo-link">
          <div class="logo-icon">Г</div>
          <span>Город <strong>Денег</strong></span>
        </router-link>
      </div>
      <h1 class="auth-title">Создать аккаунт</h1>
      <p class="auth-sub">Присоединяйтесь к сообществу</p>

      <div v-if="error" class="alert alert-danger" style="margin-top:16px">{{ error }}</div>
      <div v-if="errors.length" class="alert alert-danger" style="margin-top:16px">
        <div v-for="e in errors" :key="e">{{ e }}</div>
      </div>

      <form @submit.prevent="submit" class="auth-form">
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Имя</label>
            <input v-model="form.firstName" class="form-control" placeholder="Айбек" required />
          </div>
          <div class="form-group">
            <label class="form-label">Фамилия</label>
            <input v-model="form.lastName" class="form-control" placeholder="Джаксыбеков" required />
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">Email</label>
          <input v-model="form.email" type="email" class="form-control" placeholder="you@example.com" required />
        </div>
        <div class="form-group">
          <label class="form-label">Пароль</label>
          <div class="pw-wrap">
            <input v-model="form.password" :type="showPw?'text':'password'" class="form-control" placeholder="Минимум 8 символов" required minlength="8" />
            <button type="button" class="pw-eye" @click="showPw=!showPw">{{ showPw?'🙈':'👁' }}</button>
          </div>
          <div class="form-hint">Минимум 8 символов, заглавная буква и цифра</div>
        </div>
        <div class="terms">
          <input type="checkbox" v-model="agreed" id="terms" required />
          <label for="terms">Принимаю <a href="#">условия использования</a> и <a href="#">политику конфиденциальности</a></label>
        </div>
        <button type="submit" class="btn btn-primary btn-full" :disabled="loading||!agreed">
          <span v-if="loading" class="spinner" style="width:16px;height:16px" />
          <span v-else>Создать аккаунт</span>
        </button>
      </form>
      <div class="auth-footer">Уже есть аккаунт? <router-link to="/login">Войти</router-link></div>
    </div>
  </div>
</template>
<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
const auth = useAuthStore(); const router = useRouter()
const loading = ref(false); const error = ref(''); const errors = ref([]); const showPw = ref(false); const agreed = ref(false)
const form = reactive({ firstName:'', lastName:'', email:'', password:'' })
async function submit() {
  error.value=''; errors.value=[]; loading.value=true
  try { await auth.register({...form}); await router.push('/cabinet') }
  catch(e) { if(e.response?.data?.errors) errors.value=e.response.data.errors; else error.value=e.response?.data?.message||'Ошибка регистрации' }
  loading.value=false
}
</script>
<style scoped>
.auth-page { min-height: 100vh; display: flex; align-items: center; justify-content: center; padding: 40px 16px; background: var(--bg-soft); }
.auth-box { width: 100%; max-width: 460px; background: var(--bg); border: 1px solid var(--border); border-radius: var(--r-xl); padding: 40px; box-shadow: var(--shadow-md); }
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
.terms { display: flex; align-items: flex-start; gap: 10px; margin-bottom: 20px; font-size: 13px; color: var(--text-muted); }
.terms input { margin-top: 2px; accent-color: var(--blue); }
.terms a { color: var(--blue); }
.auth-footer { text-align: center; margin-top: 20px; font-size: 14px; color: var(--text-muted); }
.auth-footer a { color: var(--blue); font-weight: 600; }
</style>
