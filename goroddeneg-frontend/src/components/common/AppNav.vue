<template>
  <header class="nav" :class="{ scrolled }">
    <div class="container nav-inner">
      <router-link to="/" class="nav-logo">
        <div class="logo-icon">Г</div>
        <span class="logo-text">Город <strong>Денег</strong></span>
      </router-link>

      <nav class="nav-links" :class="{ open: menuOpen }">
        <router-link to="/projects"     class="nav-link" @click="menuOpen=false">Проекты</router-link>
        <router-link to="/how-it-works" class="nav-link" @click="menuOpen=false">Как работает</router-link>
        <router-link to="/about"        class="nav-link" @click="menuOpen=false">О нас</router-link>
        <router-link to="/why"          class="nav-link" @click="menuOpen=false">Почему это выгодно</router-link>
      </nav>

      <div class="nav-right">
        <template v-if="auth.isAuth">
          <div class="nav-balance">
            <span class="balance-icon">₸</span>
            {{ fmt(auth.user?.balance) }}
          </div>
          <div class="nav-user" ref="dropRef">
            <button class="user-btn" @click="drop=!drop">
              <div class="user-avatar">
                <img v-if="auth.user?.avatarUrl" :src="auth.user.avatarUrl" />
                <span v-else>{{ auth.user?.firstName?.[0] }}{{ auth.user?.lastName?.[0] }}</span>
              </div>
              <svg class="chevron" :class="{up:drop}" width="10" height="6" viewBox="0 0 10 6"><path d="M1 1l4 4 4-4" stroke="currentColor" stroke-width="1.5" fill="none" stroke-linecap="round"/></svg>
            </button>
            <transition name="fade">
              <div v-if="drop" class="dropdown">
                <div class="dropdown-user">
                  <div class="du-name">{{ auth.user?.firstName }} {{ auth.user?.lastName }}</div>
                  <div class="du-email">{{ auth.user?.email }}</div>
                </div>
                <div class="dropdown-sep" />
                <router-link to="/cabinet" class="dropdown-item" @click="drop=false">
                  <span class="di-icon">👤</span> Личный кабинет
                </router-link>
                <router-link to="/create" class="dropdown-item" @click="drop=false">
                  <span class="di-icon">✨</span> Создать проект
                </router-link>
                <router-link v-if="auth.isAdmin" to="/admin" class="dropdown-item" @click="drop=false">
                  <span class="di-icon">⚙️</span> Администрирование
                </router-link>
                <div class="dropdown-sep" />
                <button class="dropdown-item logout" @click="logout">
                  <span class="di-icon">🚪</span> Выйти
                </button>
              </div>
            </transition>
          </div>
        </template>
        <template v-else>
          <router-link to="/login"    class="btn btn-ghost btn-sm">Войти</router-link>
          <router-link to="/register" class="btn btn-primary btn-sm">Начать</router-link>
        </template>
      </div>

      <button class="burger" :class="{active:menuOpen}" @click="menuOpen=!menuOpen">
        <span/><span/><span/>
      </button>
    </div>
  </header>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth     = useAuthStore()
const router   = useRouter()
const scrolled = ref(false)
const drop     = ref(false)
const menuOpen = ref(false)
const dropRef  = ref(null)

function fmt(n) {
  if (!n && n !== 0) return '0 ₸'
  return new Intl.NumberFormat('ru-KZ', { maximumFractionDigits: 0 }).format(n) + ' ₸'
}
async function logout() {
  drop.value = false; auth.logout(); await router.push('/')
}
function onScroll() { scrolled.value = window.scrollY > 10 }
function onClickOutside(e) {
  if (dropRef.value && !dropRef.value.contains(e.target)) drop.value = false
}
onMounted(() => {
  window.addEventListener('scroll', onScroll)
  document.addEventListener('click', onClickOutside)
})
onUnmounted(() => {
  window.removeEventListener('scroll', onScroll)
  document.removeEventListener('click', onClickOutside)
})
</script>

<style scoped>
.nav {
  position: sticky; top: 0; z-index: 100; height: 68px;
  background: rgba(255,255,255,.95);
  backdrop-filter: blur(12px);
  border-bottom: 1px solid transparent;
  transition: all .2s ease;
}
.nav.scrolled {
  border-bottom-color: var(--border);
  box-shadow: 0 1px 12px rgba(26,26,46,.07);
}
.nav-inner {
  height: 68px; display: flex; align-items: center; gap: 24px;
}
.nav-logo {
  display: flex; align-items: center; gap: 10px;
  text-decoration: none; flex-shrink: 0;
}
.logo-icon {
  width: 34px; height: 34px; border-radius: 10px;
  background: var(--blue); color: #fff;
  font-family: 'Bricolage Grotesque', sans-serif;
  font-size: 18px; font-weight: 800;
  display: flex; align-items: center; justify-content: center;
}
.logo-text {
  font-family: 'Bricolage Grotesque', sans-serif;
  font-size: 17px; font-weight: 600; color: var(--text);
}
.logo-text strong { color: var(--blue); font-weight: 800; }

.nav-links { display: flex; align-items: center; gap: 2px; flex: 1; }
.nav-link {
  padding: 7px 14px; border-radius: var(--r-sm);
  font-size: 14px; font-weight: 500; color: var(--text-mid);
  text-decoration: none; transition: all .15s ease;
}
.nav-link:hover { background: var(--bg-soft); color: var(--text); text-decoration: none; }
.nav-link.router-link-active { color: var(--blue); background: var(--blue-light); font-weight: 600; }

.nav-right { display: flex; align-items: center; gap: 10px; flex-shrink: 0; }

.nav-balance {
  display: flex; align-items: center; gap: 6px;
  background: var(--green-bg); border: 1px solid #A8E6CF;
  border-radius: var(--r-full); padding: 5px 14px;
  font-size: 13px; font-weight: 700; color: var(--green);
}
.balance-icon { font-style: normal; font-weight: 800; }

.nav-user { position: relative; }
.user-btn {
  display: flex; align-items: center; gap: 8px;
  background: var(--bg-soft); border: 1.5px solid var(--border);
  border-radius: var(--r-full); padding: 4px 12px 4px 4px;
  cursor: pointer; transition: all .15s; color: var(--text-muted);
}
.user-btn:hover { border-color: var(--blue); background: var(--blue-light); }
.user-avatar {
  width: 32px; height: 32px; border-radius: 50%;
  background: var(--blue); color: #fff;
  font-size: 12px; font-weight: 700;
  display: flex; align-items: center; justify-content: center; overflow: hidden;
}
.user-avatar img { width: 100%; height: 100%; object-fit: cover; }
.chevron { transition: transform .2s; }
.chevron.up { transform: rotate(180deg); }

.dropdown {
  position: absolute; top: calc(100% + 10px); right: 0;
  background: var(--bg); border: 1px solid var(--border);
  border-radius: var(--r-lg); box-shadow: var(--shadow-lg);
  min-width: 220px; overflow: hidden; z-index: 200;
}
.dropdown-user { padding: 16px 16px 12px; }
.du-name  { font-size: 14px; font-weight: 700; color: var(--text); }
.du-email { font-size: 12px; color: var(--text-muted); margin-top: 2px; }
.dropdown-sep { height: 1px; background: var(--border); }
.dropdown-item {
  display: flex; align-items: center; gap: 10px;
  padding: 11px 16px; font-size: 14px; font-weight: 500;
  color: var(--text-mid); text-decoration: none;
  cursor: pointer; background: none; border: none;
  width: 100%; text-align: left; transition: background .12s;
  font-family: 'Inter', sans-serif;
}
.dropdown-item:hover { background: var(--bg-soft); color: var(--text); text-decoration: none; }
.dropdown-item.logout:hover { background: var(--danger-bg); color: var(--danger); }
.di-icon { font-size: 15px; width: 20px; text-align: center; }

.burger {
  display: none; flex-direction: column; gap: 5px;
  background: none; border: none; cursor: pointer; padding: 6px;
}
.burger span { display: block; width: 22px; height: 2px; background: var(--text); border-radius: 2px; transition: all .2s; }
.burger.active span:nth-child(1) { transform: translateY(7px) rotate(45deg); }
.burger.active span:nth-child(2) { opacity: 0; }
.burger.active span:nth-child(3) { transform: translateY(-7px) rotate(-45deg); }

@media (max-width: 768px) {
  .nav-links {
    display: none; position: absolute; top: 68px; left: 0; right: 0;
    background: var(--bg); border-bottom: 1px solid var(--border);
    flex-direction: column; align-items: stretch;
    padding: 12px 16px 20px; gap: 4px;
    box-shadow: var(--shadow-md);
  }
  .nav-links.open { display: flex; }
  .burger { display: flex; }
  .nav-balance { display: none; }
}
</style>
