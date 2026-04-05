<template>
  <div id="app-root">
    <AppNav />
    <main>
      <router-view v-slot="{ Component }">
        <transition name="fade" mode="out-in">
          <component :is="Component" :key="$route.fullPath" />
        </transition>
      </router-view>
    </main>
    <AppFooter />
    <AppToast />
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import AppNav    from '@/components/common/AppNav.vue'
import AppFooter from '@/components/common/AppFooter.vue'
import AppToast  from '@/components/common/AppToast.vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
onMounted(async () => { if (auth.isAuth) await auth.fetchMe() })
</script>

<style>
#app-root { display: flex; flex-direction: column; min-height: 100vh; }
main      { flex: 1; }
</style>
