import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const routes = [
  { path: '/',              name: 'Home',          component: () => import('@/views/HomeView.vue') },
  { path: '/projects',      name: 'Projects',      component: () => import('@/views/ProjectsView.vue') },
  { path: '/projects/:id',  name: 'ProjectDetail', component: () => import('@/views/ProjectDetailView.vue') },
  { path: '/how-it-works',  name: 'HowItWorks',    component: () => import('@/views/HowItWorksView.vue') },
  { path: '/about',         name: 'About',         component: () => import('@/views/AboutView.vue') },
  { path: '/why',           name: 'Why',           component: () => import('@/views/WhyView.vue') },
  { path: '/login',         name: 'Login',         component: () => import('@/views/LoginView.vue'),   meta: { guest: true } },
  { path: '/register',      name: 'Register',      component: () => import('@/views/RegisterView.vue'), meta: { guest: true } },
  { path: '/create',        name: 'CreateProject', component: () => import('@/views/CreateProjectView.vue'), meta: { auth: true } },
  { path: '/cabinet',       name: 'Cabinet',       component: () => import('@/views/CabinetView.vue'),  meta: { auth: true } },
  { path: '/admin',         name: 'Admin',         component: () => import('@/views/AdminView.vue'),    meta: { auth: true, admin: true } },
  { path: '/404',             name: 'NotFound', component: () => import('@/views/NotFoundView.vue') },
  { path: '/:pathMatch(.*)*', name: 'Catch',    component: () => import('@/views/NotFoundView.vue') },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior: () => ({ top: 0, behavior: 'smooth' }),
})

router.beforeEach((to) => {
  const auth = useAuthStore()
  if (to.meta.auth  && !auth.isAuth)  return '/login'
  if (to.meta.admin && !auth.isAdmin) return '/'
  if (to.meta.guest && auth.isAuth)   return '/cabinet'
})

export default router
