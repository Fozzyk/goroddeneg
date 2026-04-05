<template>
  <div class="page">
    <div class="container">
      <div class="cabinet-layout">
        <!-- Sidebar -->
        <aside class="cab-sidebar">
          <div class="cab-profile card card-body">
            <div class="cab-avatar-wrap">
              <img v-if="auth.user?.avatarUrl" :src="auth.user.avatarUrl" class="cab-avatar" />
              <div v-else class="cab-avatar-ph">{{ auth.user?.firstName?.[0] }}{{ auth.user?.lastName?.[0] }}</div>
              <button class="cab-avatar-edit" title="Изменить фото" @click="$refs.avatarInput.click()">📷</button>
              <input ref="avatarInput" type="file" accept="image/*" style="display:none" @change="uploadAvatar" />
            </div>
            <div class="cab-name">{{ auth.user?.firstName }} {{ auth.user?.lastName }}</div>
            <div class="cab-email text-muted">{{ auth.user?.email }}</div>

            <div class="cab-balance">
              <div class="cb-label">Баланс</div>
              <div class="cb-amount font-display">{{ fmtMoney(auth.user?.balance) }}</div>
              <button class="btn btn-primary btn-sm btn-full mt-2" @click="topUpModal = true">+ Пополнить</button>
            </div>
          </div>

          <nav class="cab-nav card card-body mt-4">
            <button v-for="tab in navItems" :key="tab.id" class="cab-nav-item" :class="{ active: activeTab === tab.id }" @click="activeTab = tab.id">
              {{ tab.icon }} {{ tab.label }}
            </button>
          </nav>

          <div class="cab-nav card card-body mt-2">
            <button class="cab-nav-item danger" @click="handleLogout">🚪 Выйти</button>
          </div>
        </aside>

        <!-- Main content -->
        <div class="cab-main">
          <!-- Profile tab -->
          <div v-if="activeTab === 'profile'">
            <h2 class="font-display mb-6">Профиль</h2>
            <div class="card card-body">
              <div v-if="profileSaved" class="alert alert-success mb-4">✅ Профиль сохранён</div>
              <div class="form-row">
                <div class="form-group"><label class="form-label">Имя</label><input v-model="profile.firstName" class="form-control" /></div>
                <div class="form-group"><label class="form-label">Фамилия</label><input v-model="profile.lastName" class="form-control" /></div>
              </div>
              <div class="form-row">
                <div class="form-group"><label class="form-label">Email</label><input :value="auth.user?.email" class="form-control" disabled style="opacity:.6" /></div>
                <div class="form-group"><label class="form-label">Телефон</label><input v-model="profile.phone" class="form-control" placeholder="+7 777 123 45 67" /></div>
              </div>
              <div class="form-row">
                <div class="form-group"><label class="form-label">Город</label>
                  <select v-model="profile.city" class="form-control">
                    <option v-for="c in cities" :key="c">{{ c }}</option>
                  </select>
                </div>
                <div class="form-group"><label class="form-label">Дата рождения</label><input v-model="profile.dateOfBirth" type="date" class="form-control" /></div>
              </div>
              <div class="form-group"><label class="form-label">Адрес</label><input v-model="profile.address" class="form-control" placeholder="г. Алматы, ул. Абая, д. 10" /></div>
              <div class="form-group"><label class="form-label">О себе</label><textarea v-model="profile.bio" class="form-control" placeholder="Расскажите немного о себе..." /></div>
              <div class="form-group"><label class="form-label">Веб-сайт</label><input v-model="profile.webSite" class="form-control" placeholder="https://yoursite.com" /></div>
              <button class="btn btn-primary" :disabled="savingProfile" @click="saveProfile">
                <span v-if="savingProfile" class="spinner" style="width:16px;height:16px" />
                <span v-else>Сохранить изменения</span>
              </button>
            </div>

            <div class="card card-body mt-6">
              <h3 class="font-display mb-4">Изменить пароль</h3>
              <div v-if="pwError"   class="alert alert-danger mb-4">{{ pwError }}</div>
              <div v-if="pwSuccess" class="alert alert-success mb-4">✅ Пароль изменён</div>
              <div class="form-group"><label class="form-label">Текущий пароль</label><input v-model="pw.current" type="password" class="form-control" /></div>
              <div class="form-group"><label class="form-label">Новый пароль</label><input v-model="pw.newPw" type="password" class="form-control" /></div>
              <button class="btn btn-outline" @click="changePassword">Сменить пароль</button>
            </div>
          </div>

          <!-- Projects tab -->
          <div v-if="activeTab === 'projects'">
            <div class="tab-header">
              <h2 class="font-display">Мои проекты</h2>
              <router-link to="/create" class="btn btn-primary btn-sm">+ Создать проект</router-link>
            </div>
            <div v-if="loadingProjects" class="flex-center" style="height:200px"><div class="spinner" /></div>
            <div v-else-if="myProjects.length" class="my-projects">
              <div v-for="p in myProjects" :key="p.id" class="my-proj-row card card-body">
                <div class="mpr-cover">
                  <img v-if="p.coverImageUrl" :src="p.coverImageUrl" />
                  <div v-else class="mpr-cover-ph">{{ p.categoryIcon }}</div>
                </div>
                <div class="mpr-info">
                  <div class="mpr-title">{{ p.title }}</div>
                  <div class="mpr-meta text-muted">{{ p.categoryName }} · {{ p.city }}</div>
                  <div class="progress mt-3 mb-2"><div class="progress-fill" :style="{ width: Math.min(100,p.percentFunded)+'%' }" /></div>
                  <div style="font-size:12px;color:var(--text-muted)">{{ fmtMoney(p.collectedAmount) }} из {{ fmtMoney(p.goalAmount) }}</div>
                </div>
                <div class="mpr-right">
                  <span :class="statusClass(p.status)" class="badge">{{ statusLabel(p.status) }}</span>
                  <router-link :to="`/projects/${p.id}`" class="btn btn-ghost btn-sm mt-3">Просмотр</router-link>
                </div>
              </div>
            </div>
            <div v-else class="empty-state">
              <div style="font-size:48px;margin-bottom:16px">📁</div>
              <h3>Проектов пока нет</h3>
              <p class="text-muted mb-4">Создайте свой первый проект прямо сейчас</p>
              <router-link to="/create" class="btn btn-primary">Создать проект</router-link>
            </div>
          </div>

          <!-- Transactions tab -->
          <div v-if="activeTab === 'transactions'">
            <h2 class="font-display mb-6">История операций</h2>
            <div v-if="loadingTx" class="flex-center" style="height:200px"><div class="spinner" /></div>
            <div v-else class="card">
              <table class="table">
                <thead>
                  <tr><th>ID</th><th>Тип</th><th>Сумма</th><th>Баланс после</th><th>Примечание</th><th>Дата</th></tr>
                </thead>
                <tbody>
                  <tr v-for="t in transactions" :key="t.id">
                    <td style="font-family:monospace;font-size:12px;color:var(--text-muted)">#{{ t.id }}</td>
                    <td><span :class="txBadge(t.type)">{{ txLabel(t.type) }}</span></td>
                    <td :class="t.type==='TopUp'?'text-success':'text-danger'" style="font-weight:700">
                      {{ t.type==='TopUp' ? '+' : '−' }}{{ fmtMoney(Math.abs(t.amount)) }}
                    </td>
                    <td class="text-gold" style="font-weight:600">{{ fmtMoney(t.balanceAfter) }}</td>
                    <td style="font-size:13px;color:var(--text-muted)">{{ t.note }}</td>
                    <td style="font-size:13px;color:var(--text-muted);white-space:nowrap">{{ fmtDate(t.createdAt) }}</td>
                  </tr>
                  <tr v-if="!transactions.length"><td colspan="6" style="text-align:center;padding:40px;color:var(--text-muted)">Операций пока нет</td></tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Pledges tab -->
          <div v-if="activeTab === 'pledges'">
            <h2 class="font-display mb-6">Мои поддержки</h2>
            <div class="card">
              <table class="table">
                <thead><tr><th>Проект</th><th>Вознаграждение</th><th>Сумма</th><th>Статус</th><th>Дата</th></tr></thead>
                <tbody>
                  <tr v-for="pl in pledges" :key="pl.id">
                    <td style="font-weight:600">{{ pl.projectTitle }}</td>
                    <td style="font-size:13px;color:var(--text-muted)">{{ pl.rewardTitle || '—' }}</td>
                    <td class="text-gold" style="font-weight:700">{{ fmtMoney(pl.amount) }}</td>
                    <td><span class="badge badge-success">Оплачено</span></td>
                    <td style="font-size:13px;color:var(--text-muted)">{{ fmtDate(pl.createdAt) }}</td>
                  </tr>
                  <tr v-if="!pledges.length"><td colspan="5" style="text-align:center;padding:40px;color:var(--text-muted)">Поддержек пока нет</td></tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Top-up modal -->
    <transition name="fade">
      <div v-if="topUpModal" class="modal-overlay" @click.self="topUpModal = false">
        <div class="modal-box">
          <div class="modal-header">
            <h2>Пополнить баланс</h2>
            <button class="modal-close" @click="topUpModal = false">✕</button>
          </div>
          <div class="modal-body">
            <div v-if="topUpError" class="alert alert-danger mb-4">{{ topUpError }}</div>
            <div v-if="topUpSuccess" class="alert alert-success mb-4">✅ Баланс пополнен!</div>
            <div class="form-group">
              <label class="form-label">Сумма (₸)</label>
              <input v-model.number="topUpAmount" type="number" class="form-control" placeholder="10 000" min="100" />
            </div>
            <div class="quick-amounts">
              <button v-for="a in [5000,10000,25000,50000]" :key="a" class="quick-btn" @click="topUpAmount = a">{{ fmtMoney(a) }}</button>
            </div>
            <div class="form-hint">Демо-режим: средства добавятся на баланс мгновенно</div>
            <button class="btn btn-primary btn-full mt-4" :disabled="!topUpAmount || topUpLoading" @click="doTopUp">
              <span v-if="topUpLoading" class="spinner" style="width:16px;height:16px" />
              <span v-else>Пополнить {{ topUpAmount ? fmtMoney(topUpAmount) : '' }}</span>
            </button>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { usersApi, projectsApi } from '@/services/api'

const auth   = useAuthStore()
const router = useRouter()

const activeTab     = ref('profile')
const myProjects    = ref([])
const transactions  = ref([])
const pledges       = ref([])
const loadingProjects = ref(false)
const loadingTx     = ref(false)
const profileSaved  = ref(false)
const savingProfile = ref(false)
const pwError    = ref('')
const pwSuccess  = ref(false)
const topUpModal  = ref(false)
const topUpAmount = ref(null)
const topUpLoading = ref(false)
const topUpError  = ref('')
const topUpSuccess = ref(false)

const cities = ['Алматы','Нур-Султан','Шымкент','Актобе','Тараз','Другой']
const navItems = [
  { id:'profile',      icon:'👤', label:'Профиль' },
  { id:'projects',     icon:'📁', label:'Мои проекты' },
  { id:'transactions', icon:'📊', label:'История операций' },
  { id:'pledges',      icon:'💛', label:'Мои поддержки' },
]

const profile = reactive({
  firstName: auth.user?.firstName || '',
  lastName:  auth.user?.lastName  || '',
  phone:     auth.user?.phone     || '',
  city:      auth.user?.city      || '',
  address:   auth.user?.address   || '',
  bio:       auth.user?.bio       || '',
  webSite:   auth.user?.webSite   || '',
  dateOfBirth: auth.user?.dateOfBirth?.split('T')[0] || '',
})
const pw = reactive({ current: '', newPw: '' })

async function saveProfile() {
  savingProfile.value = true
  try {
    const { data } = await usersApi.updateProfile({ ...profile })
    auth.updateUser(data)
    profileSaved.value = true
    setTimeout(() => profileSaved.value = false, 3000)
  } catch {}
  savingProfile.value = false
}

async function changePassword() {
  pwError.value = ''; pwSuccess.value = false
  try {
    await usersApi.changePassword({ currentPassword: pw.current, newPassword: pw.newPw })
    pwSuccess.value = true; pw.current = ''; pw.newPw = ''
  } catch (e) {
    pwError.value = e.response?.data?.message || 'Ошибка смены пароля'
  }
}

async function uploadAvatar(e) {
  const file = e.target.files[0]; if (!file) return
  try {
    const { data } = await usersApi.uploadAvatar(file)
    auth.updateUser({ avatarUrl: data.url })
  } catch {}
}

async function doTopUp() {
  topUpLoading.value = true; topUpError.value = ''
  try {
    const { data } = await usersApi.topUp({ amount: topUpAmount.value, paymentRef: `DEMO-${Date.now()}` })
    auth.updateUser({ balance: data.newBalance })
    topUpSuccess.value = true
    setTimeout(() => { topUpModal.value = false; topUpSuccess.value = false; topUpAmount.value = null }, 1500)
  } catch (e) { topUpError.value = e.response?.data?.message || 'Ошибка пополнения' }
  topUpLoading.value = false
}

async function handleLogout() { auth.logout(); await router.push('/') }

watch(activeTab, async (tab) => {
  if (tab === 'projects' && !myProjects.value.length) {
    loadingProjects.value = true
    try { const { data } = await projectsApi.getMy(); myProjects.value = data } catch {}
    loadingProjects.value = false
  }
  if (tab === 'transactions' && !transactions.value.length) {
    loadingTx.value = true
    try { const { data } = await usersApi.getTransactions(); transactions.value = data.items } catch {}
    loadingTx.value = false
  }
  if (tab === 'pledges' && !pledges.value.length) {
    try { const { data } = await usersApi.getPledges(); pledges.value = data } catch {}
  }
}, { immediate: false })

function fmtMoney(n) { return new Intl.NumberFormat('ru-KZ', { maximumFractionDigits:0 }).format(n||0) + ' ₸' }
function fmtDate(d)   { return new Date(d).toLocaleDateString('ru-KZ', { day:'numeric', month:'short', year:'numeric' }) }
function statusLabel(s) { return { Draft:'Черновик', Pending:'На модерации', Active:'Активный', Rejected:'Отклонён', Completed:'Завершён', Failed:'Не состоялся' }[s] || s }
function statusClass(s) { return { Draft:'badge-muted', Pending:'badge-warning', Active:'badge-success', Rejected:'badge-danger', Completed:'badge-info' }[s] || 'badge-muted' }
function txLabel(t) { return { TopUp:'Пополнение', PledgeOut:'Поддержка', Refund:'Возврат', PlatformFee:'Комиссия' }[t] || t }
function txBadge(t) { return t === 'TopUp' ? 'badge badge-success' : t === 'Refund' ? 'badge badge-info' : 'badge badge-gold' }
</script>

<style scoped>
.cabinet-layout { display: grid; grid-template-columns: 260px 1fr; gap: 32px; align-items: start; }
.cab-profile { text-align: center; }
.cab-avatar-wrap { position: relative; width: 88px; margin: 0 auto 16px; }
.cab-avatar    { width: 88px; height: 88px; border-radius: 50%; object-fit: cover; border: 3px solid var(--gold-pale); }
.cab-avatar-ph { width: 88px; height: 88px; border-radius: 50%; background: linear-gradient(135deg, var(--gold), var(--gold-light)); color: #fff; font-size: 30px; font-weight: 700; display: flex; align-items: center; justify-content: center; }
.cab-avatar-edit { position: absolute; bottom: 0; right: 0; width: 28px; height: 28px; border-radius: 50%; background: var(--warm-white); border: 1.5px solid var(--border); font-size: 13px; cursor: pointer; display: flex; align-items: center; justify-content: center; }
.cab-name  { font-weight: 700; font-size: 16px; }
.cab-email { font-size: 13px; margin-top: 4px; margin-bottom: 20px; }
.cab-balance { background: var(--gold-subtle); border: 1px solid var(--border); border-radius: var(--r-sm); padding: 16px; }
.cb-label  { font-size: 11px; font-weight: 700; text-transform: uppercase; letter-spacing: .07em; color: var(--text-muted); margin-bottom: 6px; }
.cb-amount { font-size: 1.8rem; color: var(--gold); }

.cab-nav { padding: 8px; }
.cab-nav-item { width: 100%; text-align: left; padding: 11px 14px; border-radius: var(--r-sm); border: none; background: none; font-size: 14px; font-weight: 500; color: var(--text-secondary); cursor: pointer; transition: all .18s; font-family: 'DM Sans', sans-serif; margin-bottom: 2px; }
.cab-nav-item:hover  { background: var(--gold-subtle); color: var(--gold); }
.cab-nav-item.active { background: var(--gold-pale); color: var(--gold); font-weight: 700; }
.cab-nav-item.danger:hover { background: var(--danger-bg); color: var(--danger); }

.cab-main { min-width: 0; }
.tab-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }

.my-proj-row { display: flex; align-items: center; gap: 20px; margin-bottom: 14px; }
.mpr-cover { width: 80px; height: 64px; border-radius: var(--r-sm); overflow: hidden; flex-shrink: 0; background: var(--sand); }
.mpr-cover img { width: 100%; height: 100%; object-fit: cover; }
.mpr-cover-ph  { width: 100%; height: 100%; display: flex; align-items: center; justify-content: center; font-size: 24px; }
.mpr-info  { flex: 1; min-width: 0; }
.mpr-title { font-weight: 700; font-size: 15px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.mpr-meta  { font-size: 12px; margin-top: 3px; }
.mpr-right { text-align: right; flex-shrink: 0; }

.quick-amounts { display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 8px; }
.quick-btn { padding: 7px 14px; border-radius: var(--r-sm); border: 1px solid var(--border); background: var(--sand); font-size: 13px; font-weight: 600; cursor: pointer; color: var(--text-secondary); font-family: 'DM Sans', sans-serif; transition: all .18s; }
.quick-btn:hover { border-color: var(--gold); color: var(--gold); }

.empty-state { text-align: center; padding: 60px 24px; }

@media (max-width: 768px) { .cabinet-layout { grid-template-columns: 1fr; } }
</style>
