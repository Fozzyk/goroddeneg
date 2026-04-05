<template>
  <div class="page">
    <div class="container" style="max-width:1280px">
      <div class="admin-layout">
        <!-- Sidebar -->
        <aside class="admin-sidebar card">
          <div class="admin-brand">
            <div style="font-size:28px">⚙️</div>
            <div>
              <div style="font-weight:700;font-size:15px">Администрация</div>
              <div class="text-muted" style="font-size:12px">{{ auth.user?.email }}</div>
            </div>
          </div>
          <nav class="admin-nav">
            <button v-for="item in navItems" :key="item.id" class="admin-nav-item" :class="{ active: activeTab === item.id }" @click="activeTab = item.id; loadTab(item.id)">
              {{ item.icon }} {{ item.label }}
            </button>
          </nav>
        </aside>

        <!-- Content -->
        <div class="admin-content">
          <!-- Stats -->
          <div v-if="activeTab === 'dashboard'">
            <h2 class="font-display mb-6">Дашборд</h2>
            <div class="stats-grid" v-if="stats">
              <div class="stat-tile card card-body">
                <div class="st-icon">📂</div>
                <div class="st-val font-display">{{ stats.activeProjects }}</div>
                <div class="st-lbl">Активных проектов</div>
              </div>
              <div class="stat-tile card card-body">
                <div class="st-icon">⏳</div>
                <div class="st-val font-display">{{ stats.pendingProjects }}</div>
                <div class="st-lbl">На модерации</div>
              </div>
              <div class="stat-tile card card-body">
                <div class="st-icon">👥</div>
                <div class="st-val font-display">{{ stats.totalUsers }}</div>
                <div class="st-lbl">Пользователей</div>
              </div>
              <div class="stat-tile card card-body">
                <div class="st-icon">💰</div>
                <div class="st-val font-display text-gold">{{ fmtMoney(stats.monthlyRevenue) }}</div>
                <div class="st-lbl">Выручка за месяц</div>
              </div>
            </div>
            <div v-else class="flex-center" style="height:160px"><div class="spinner" /></div>
          </div>

          <!-- Projects -->
          <div v-if="activeTab === 'projects'">
            <div class="tab-header mb-6">
              <h2 class="font-display">Проекты</h2>
              <div class="flex gap-2">
                <select v-model="projFilter" class="form-control" style="width:180px" @change="loadProjects">
                  <option value="">Все статусы</option>
                  <option value="Pending">На модерации</option>
                  <option value="Active">Активные</option>
                  <option value="Rejected">Отклонённые</option>
                  <option value="Completed">Завершённые</option>
                  <option value="Draft">Черновики</option>
                </select>
              </div>
            </div>

            <div v-if="loadingProjects" class="flex-center" style="height:200px"><div class="spinner" /></div>
            <div v-else>
              <!-- Pending highlighted -->
              <div v-if="!projFilter || projFilter === 'Pending'">
                <div v-if="pendingProjects.length" class="pending-section">
                  <div class="section-badge">⚠️ Ожидают модерации: {{ pendingProjects.length }}</div>
                  <div v-for="p in pendingProjects" :key="p.id" class="proj-row card card-body pending">
                    <div class="pr-info">
                      <div class="pr-title">{{ p.title }}</div>
                      <div class="pr-meta text-muted">{{ p.categoryName }} · {{ p.authorEmail }} · {{ fmtDate(p.createdAt) }}</div>
                      <div class="pr-amounts"><span class="text-gold">{{ fmtMoney(p.goalAmount) }}</span> · {{ p.percentFunded }}% собрано</div>
                    </div>
                    <div class="pr-actions">
                      <button class="btn btn-sm" style="background:var(--success-bg);color:var(--success)" @click="updateStatus(p.id,'Active')">✓ Одобрить</button>
                      <button class="btn btn-sm" style="background:var(--danger-bg);color:var(--danger);margin-top:6px" @click="openRejectModal(p)">✕ Отклонить</button>
                    </div>
                  </div>
                </div>
              </div>

              <!-- All projects table -->
              <div class="card mt-4">
                <table class="table">
                  <thead>
                    <tr>
                      <th>ID</th><th>Название</th><th>Автор</th><th>Категория</th>
                      <th>Сбор</th><th>Статус</th><th>Действие</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="p in allProjects" :key="p.id">
                      <td style="font-size:12px;color:var(--text-muted)">#{{ p.id }}</td>
                      <td style="font-weight:600;max-width:200px">{{ p.title }}</td>
                      <td style="font-size:13px;color:var(--text-muted)">{{ p.authorEmail }}</td>
                      <td style="font-size:13px">{{ p.categoryName }}</td>
                      <td class="text-gold" style="font-weight:700">{{ fmtMoney(p.collectedAmount) }}</td>
                      <td><span :class="statusBadge(p.status)">{{ statusLabel(p.status) }}</span></td>
                      <td>
                        <div class="flex gap-1" style="flex-wrap:wrap">
                          <button v-if="p.status==='Pending'" class="btn btn-sm" style="background:var(--success-bg);color:var(--success)" @click="updateStatus(p.id,'Active')">Одобрить</button>
                          <button v-if="p.status==='Active'" class="btn btn-sm" style="background:var(--danger-bg);color:var(--danger)" @click="updateStatus(p.id,'Completed')">Завершить</button>
                          <button v-if="['Pending','Draft'].includes(p.status)" class="btn btn-sm" style="background:var(--danger-bg);color:var(--danger)" @click="openRejectModal(p)">Отклонить</button>
                        </div>
                      </td>
                    </tr>
                    <tr v-if="!allProjects.length"><td colspan="7" style="text-align:center;padding:40px;color:var(--text-muted)">Нет проектов</td></tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <!-- Users -->
          <div v-if="activeTab === 'users'">
            <h2 class="font-display mb-6">Пользователи</h2>
            <div class="card">
              <table class="table">
                <thead><tr><th>ID</th><th>Email</th><th>Имя</th><th>Баланс</th><th>Email подтверждён</th><th>Дата регистрации</th></tr></thead>
                <tbody>
                  <tr v-for="u in users" :key="u.id">
                    <td style="font-size:12px;color:var(--text-muted)">#{{ u.id }}</td>
                    <td>{{ u.email }}</td>
                    <td>{{ u.firstName }} {{ u.lastName }}</td>
                    <td class="text-gold" style="font-weight:600">{{ fmtMoney(u.balance) }}</td>
                    <td><span :class="u.emailConfirmed?'badge badge-success':'badge badge-muted'">{{ u.emailConfirmed?'Да':'Нет' }}</span></td>
                    <td style="font-size:13px;color:var(--text-muted)">{{ fmtDate(u.createdAt) }}</td>
                  </tr>
                  <tr v-if="!users.length"><td colspan="6" style="text-align:center;padding:40px;color:var(--text-muted)">Пользователей нет</td></tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Transactions -->
          <div v-if="activeTab === 'transactions'">
            <h2 class="font-display mb-6">Транзакции</h2>
            <div class="card">
              <table class="table">
                <thead><tr><th>ID</th><th>Email</th><th>Тип</th><th>Сумма</th><th>Примечание</th><th>Дата</th></tr></thead>
                <tbody>
                  <tr v-for="t in adminTx" :key="t.id">
                    <td style="font-size:12px;font-family:monospace;color:var(--text-muted)">#{{ t.id }}</td>
                    <td style="font-size:13px">{{ t.userEmail }}</td>
                    <td><span :class="txBadge(t.type)">{{ txLabel(t.type) }}</span></td>
                    <td class="text-gold" style="font-weight:700">{{ fmtMoney(t.amount) }}</td>
                    <td style="font-size:13px;color:var(--text-muted)">{{ t.note }}</td>
                    <td style="font-size:13px;color:var(--text-muted)">{{ fmtDate(t.createdAt) }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Reject modal -->
    <transition name="fade">
      <div v-if="rejectModal" class="modal-overlay" @click.self="rejectModal = false">
        <div class="modal-box">
          <div class="modal-header">
            <h2>Отклонить проект</h2>
            <button class="modal-close" @click="rejectModal = false">✕</button>
          </div>
          <div class="modal-body">
            <p class="text-muted mb-4">Проект: <strong>{{ rejectTarget?.title }}</strong></p>
            <div class="form-group">
              <label class="form-label">Причина отклонения (будет отправлена автору)</label>
              <textarea v-model="rejectNote" class="form-control" placeholder="Опишите причину..." />
            </div>
            <button class="btn btn-danger btn-full" @click="doReject">Отклонить проект</button>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { adminApi } from '@/services/api'

const auth = useAuthStore()
const activeTab      = ref('dashboard')
const stats          = ref(null)
const allProjects    = ref([])
const users          = ref([])
const adminTx        = ref([])
const loadingProjects = ref(false)
const projFilter     = ref('')
const rejectModal    = ref(false)
const rejectTarget   = ref(null)
const rejectNote     = ref('')

const pendingProjects = computed(() => allProjects.value.filter(p => p.status === 'Pending'))

const navItems = [
  { id:'dashboard',    icon:'📊', label:'Дашборд' },
  { id:'projects',     icon:'📂', label:'Проекты' },
  { id:'users',        icon:'👥', label:'Пользователи' },
  { id:'transactions', icon:'💳', label:'Транзакции' },
]

onMounted(async () => {
  try { const { data } = await adminApi.getStats(); stats.value = data } catch {}
  await loadProjects()
})

async function loadTab(tab) {
  if (tab === 'projects')     await loadProjects()
  if (tab === 'users')        await loadUsers()
  if (tab === 'transactions') await loadAdminTx()
}

async function loadProjects() {
  loadingProjects.value = true
  try {
    const { data } = await adminApi.getProjects({ status: projFilter.value || undefined })
    allProjects.value = data.items
  } catch {}
  loadingProjects.value = false
}

async function loadUsers() {
  try { const { data } = await adminApi.getUsers(); users.value = data.items } catch {}
}

async function loadAdminTx() {
  try { const { data } = await adminApi.getTransactions(); adminTx.value = data.items } catch {}
}

async function updateStatus(id, status, note = '') {
  try {
    await adminApi.updateProjectStatus(id, { status, adminNote: note })
    await loadProjects()
    try { const { data } = await adminApi.getStats(); stats.value = data } catch {}
  } catch {}
}

function openRejectModal(p) { rejectTarget.value = p; rejectNote.value = ''; rejectModal.value = true }
async function doReject() {
  await updateStatus(rejectTarget.value.id, 'Rejected', rejectNote.value)
  rejectModal.value = false
}

function fmtMoney(n) { return new Intl.NumberFormat('ru-KZ', { maximumFractionDigits:0 }).format(n||0) + ' ₸' }
function fmtDate(d)   { return new Date(d).toLocaleDateString('ru-KZ', { day:'numeric', month:'short', year:'numeric' }) }
function statusLabel(s) { return { Draft:'Черновик', Pending:'Модерация', Active:'Активный', Rejected:'Отклонён', Completed:'Завершён', Failed:'Не состоялся' }[s] || s }
function statusBadge(s) { return { Draft:'badge badge-muted', Pending:'badge badge-warning', Active:'badge badge-success', Rejected:'badge badge-danger', Completed:'badge badge-info' }[s] || 'badge badge-muted' }
function txLabel(t) { return { TopUp:'Пополнение', PledgeOut:'Поддержка', Refund:'Возврат', PlatformFee:'Комиссия' }[t] || t }
function txBadge(t) { return t==='TopUp' ? 'badge badge-success' : t==='Refund' ? 'badge badge-info' : 'badge badge-gold' }
</script>

<style scoped>
.admin-layout { display: grid; grid-template-columns: 220px 1fr; gap: 28px; align-items: start; }
.admin-brand  { display: flex; align-items: center; gap: 12px; padding: 20px; border-bottom: 1px solid var(--border); }
.admin-nav    { padding: 8px; }
.admin-nav-item { width: 100%; text-align: left; padding: 11px 14px; border-radius: var(--r-sm); border: none; background: none; font-size: 14px; font-weight: 500; color: var(--text-secondary); cursor: pointer; transition: all .18s; font-family: 'DM Sans', sans-serif; margin-bottom: 2px; display: block; }
.admin-nav-item:hover  { background: var(--gold-subtle); color: var(--gold); }
.admin-nav-item.active { background: var(--gold-pale); color: var(--gold); font-weight: 700; }

.stats-grid { display: grid; grid-template-columns: repeat(4,1fr); gap: 20px; }
.stat-tile   { text-align: center; }
.st-icon { font-size: 36px; margin-bottom: 12px; }
.st-val  { font-size: 2.2rem; color: var(--text-primary); }
.st-lbl  { font-size: 13px; color: var(--text-muted); margin-top: 4px; }

.pending-section { margin-bottom: 20px; }
.section-badge { display: inline-flex; align-items: center; gap: 6px; background: var(--warning-bg); color: var(--warning); border: 1px solid rgba(160,112,0,.2); border-radius: var(--r-full); padding: 6px 16px; font-size: 13px; font-weight: 700; margin-bottom: 14px; }

.proj-row { display: flex; align-items: center; gap: 20px; margin-bottom: 12px; }
.proj-row.pending { border-color: rgba(160,112,0,.3); background: var(--warning-bg); }
.pr-info  { flex: 1; }
.pr-title { font-weight: 700; font-size: 15px; margin-bottom: 4px; }
.pr-meta  { font-size: 12px; margin-bottom: 6px; }
.pr-amounts { font-size: 13px; }
.pr-actions { display: flex; flex-direction: column; flex-shrink: 0; }

.tab-header { display: flex; justify-content: space-between; align-items: center; }

@media (max-width: 960px) {
  .admin-layout { grid-template-columns: 1fr; }
  .stats-grid   { grid-template-columns: 1fr 1fr; }
}
@media (max-width: 540px) { .stats-grid { grid-template-columns: 1fr; } }
</style>
