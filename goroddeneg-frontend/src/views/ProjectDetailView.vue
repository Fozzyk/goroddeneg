<template>
  <div class="page" v-if="!loading">
    <div class="container">
      <button class="back-btn" @click="$router.back()">← Назад к проектам</button>

      <div class="detail-layout">
        <!-- Left: main content -->
        <div class="detail-main">
          <!-- Cover -->
          <div class="detail-cover">
            <img v-if="project.coverImageUrl" :src="project.coverImageUrl" :alt="project.title" />
            <div v-else class="cover-placeholder">{{ project.categoryIcon }}</div>
          </div>

          <!-- Meta -->
          <div class="detail-meta">
            <span class="badge badge-gold">{{ project.categoryName }}</span>
            <span class="meta-city">📍 {{ project.city }}</span>
            <span class="meta-views">👁 {{ project.viewsCount }} просмотров</span>
          </div>

          <h1 class="detail-title">{{ project.title }}</h1>
          <p class="detail-short">{{ project.shortDesc }}</p>

          <!-- Tabs -->
          <div class="tabs">
            <button v-for="tab in tabs" :key="tab" class="tab-btn" :class="{ active: activeTab === tab }" @click="activeTab = tab">{{ tab }}</button>
          </div>

          <!-- Tab: О проекте -->
          <div v-if="activeTab === 'О проекте'">
            <div v-if="project.videoUrl" class="video-wrap">
              <iframe :src="embedUrl(project.videoUrl)" allowfullscreen />
            </div>
            <div class="detail-body" v-html="nl2br(project.fullDesc || project.shortDesc)" />

            <!-- Media gallery -->
            <div v-if="project.media?.length" class="media-grid">
              <img v-for="m in project.media.filter(m=>m.mediaType==='image')" :key="m.id" :src="m.url" class="media-img" />
            </div>
          </div>

          <!-- Tab: Автор -->
          <div v-if="activeTab === 'Автор'" class="author-tab">
            <div class="author-profile">
              <img v-if="project.authorAvatar" :src="project.authorAvatar" class="author-avatar" />
              <div v-else class="author-avatar-ph">{{ project.authorName?.[0] }}</div>
              <div>
                <div class="author-name">{{ project.authorName }}</div>
                <div class="author-bio text-muted mt-2" v-if="project.authorBio">{{ project.authorBio }}</div>
              </div>
            </div>
          </div>

          <!-- Tab: Обновления -->
          <div v-if="activeTab === 'Обновления'">
            <div v-if="project.updates?.length" class="updates-list">
              <div v-for="u in project.updates" :key="u.id" class="update-card">
                <div class="update-date">{{ fmtDate(u.createdAt) }}</div>
                <h3 class="update-title">{{ u.title }}</h3>
                <p class="update-body text-muted">{{ u.body }}</p>
              </div>
            </div>
            <div v-else class="empty-state">
              <div class="empty-icon">📭</div>
              <p>Обновлений пока нет</p>
            </div>
          </div>
        </div>

        <!-- Right: support sidebar -->
        <aside class="detail-sidebar">
          <div class="support-card card">
            <div class="card-body">
              <!-- Amount -->
              <div class="support-amount font-display">{{ fmtMoney(project.collectedAmount) }}</div>
              <div class="support-goal text-muted">из {{ fmtMoney(project.goalAmount) }}</div>

              <!-- Progress -->
              <div class="progress progress-lg mt-4 mb-3">
                <div class="progress-fill" :style="{ width: pct + '%' }" />
              </div>

              <!-- Stats row -->
              <div class="support-stats">
                <div class="ss-item">
                  <div class="ss-val">{{ pct }}%</div>
                  <div class="ss-lbl">выполнено</div>
                </div>
                <div class="ss-sep" />
                <div class="ss-item">
                  <div class="ss-val">{{ project.backersCount }}</div>
                  <div class="ss-lbl">спонсоров</div>
                </div>
                <div class="ss-sep" />
                <div class="ss-item">
                  <div class="ss-val">{{ project.daysLeft > 0 ? project.daysLeft : 0 }}</div>
                  <div class="ss-lbl">{{ project.daysLeft > 0 ? 'дней' : 'завершён' }}</div>
                </div>
              </div>

              <button class="btn btn-primary btn-full mt-6" style="font-size:16px;padding:14px" @click="openPledgeModal()">
                💛 Поддержать проект
              </button>

              <!-- Rewards -->
              <div v-if="project.rewards?.length" class="rewards-section">
                <div class="rewards-title">Варианты поддержки</div>
                <div
                  v-for="r in project.rewards"
                  :key="r.id"
                  class="reward-opt"
                  :class="{ selected: selectedReward?.id === r.id }"
                  @click="selectedReward = r; openPledgeModal(r)"
                >
                  <div class="ro-amount">{{ fmtMoney(r.minAmount) }}</div>
                  <div class="ro-title">{{ r.title }}</div>
                  <div class="ro-desc text-muted">{{ r.description }}</div>
                  <div class="ro-delivery" v-if="r.deliveryMonth">
                    📦 Доставка: {{ monthName(r.deliveryMonth) }} {{ r.deliveryYear }}
                  </div>
                  <div class="ro-limit" v-if="r.limitCount">
                    {{ r.claimedCount }}/{{ r.limitCount }} взято
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Share -->
          <div class="share-card card card-body mt-4">
            <div class="share-title">Поделиться проектом</div>
            <div class="share-btns">
              <button class="share-btn" @click="share('telegram')">✈️ Telegram</button>
              <button class="share-btn" @click="share('whatsapp')">💬 WhatsApp</button>
              <button class="share-btn" @click="copyLink">🔗 Ссылка</button>
            </div>
            <div v-if="copied" class="alert alert-success mt-2">Ссылка скопирована!</div>
          </div>
        </aside>
      </div>
    </div>

    <!-- Pledge modal -->
    <transition name="fade">
      <div v-if="pledgeModal" class="modal-overlay" @click.self="pledgeModal = false">
        <div class="modal-box">
          <div class="modal-header">
            <h2>Поддержка проекта</h2>
            <button class="modal-close" @click="pledgeModal = false">✕</button>
          </div>
          <div class="modal-body">
            <template v-if="pledgeStep === 0">
              <div class="form-group">
                <label class="form-label">Вознаграждение</label>
                <input class="form-control" readonly :value="selectedReward ? selectedReward.title : 'Без вознаграждения'" />
              </div>
              <div class="form-group">
                <label class="form-label">Сумма поддержки (₸)</label>
                <input v-model.number="pledgeAmount" type="number" class="form-control" :placeholder="`Минимум ${selectedReward ? selectedReward.minAmount : 100} ₸`" />
              </div>
              <div v-if="auth.isAuth" class="alert alert-gold mb-4">
                💰 Ваш баланс: {{ fmtMoney(auth.user?.balance) }}
              </div>
              <div v-else class="alert alert-info mb-4">
                Для поддержки проекта необходимо <router-link to="/login">войти</router-link>
              </div>
              <div v-if="pledgeError" class="alert alert-danger mb-4">{{ pledgeError }}</div>
              <button
                class="btn btn-primary btn-full"
                :disabled="!auth.isAuth || pledgeLoading"
                @click="submitPledge"
              >
                <span v-if="pledgeLoading" class="spinner" style="width:18px;height:18px" />
                <span v-else>Оплатить {{ fmtMoney(pledgeAmount) }}</span>
              </button>
              <p class="form-hint text-center mt-2">
                Нажимая «Оплатить», вы принимаете <a href="#">Пользовательское соглашение</a>
              </p>
            </template>

            <template v-if="pledgeStep === 1">
              <div style="text-align:center;padding:24px 0">
                <div style="font-size:60px;margin-bottom:16px">🎉</div>
                <h3 class="font-display" style="margin-bottom:8px">Спасибо за поддержку!</h3>
                <p class="text-muted">Ваш вклад уже помогает воплощать эту идею в жизнь.</p>
                <p class="mt-4 text-muted">Новый баланс: <strong class="text-gold">{{ fmtMoney(newBalance) }}</strong></p>
                <button class="btn btn-primary mt-6" @click="pledgeModal = false; pledgeStep = 0">Отлично!</button>
              </div>
            </template>
          </div>
        </div>
      </div>
    </transition>
  </div>

  <div v-else class="page flex-center">
    <div class="spinner" style="width:40px;height:40px" />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { projectsApi } from '@/services/api'

const route = useRoute()
const auth  = useAuthStore()

const project       = ref({})
const loading       = ref(true)
const activeTab     = ref('О проекте')
const selectedReward = ref(null)
const pledgeModal   = ref(false)
const pledgeStep    = ref(0)
const pledgeAmount  = ref(0)
const pledgeLoading = ref(false)
const pledgeError   = ref('')
const newBalance    = ref(0)
const copied        = ref(false)
const tabs = ['О проекте', 'Автор', 'Обновления']

const pct = computed(() =>
  project.value.goalAmount > 0
    ? Math.min(100, Math.round(project.value.collectedAmount * 100 / project.value.goalAmount))
    : 0
)

onMounted(async () => {
  try {
    const { data } = await projectsApi.getById(route.params.id)
    project.value = data
  } catch { }
  loading.value = false
})

function openPledgeModal(reward = null) {
  selectedReward.value = reward
  pledgeAmount.value   = reward ? reward.minAmount : 1000
  pledgeError.value    = ''
  pledgeStep.value     = 0
  pledgeModal.value    = true
}

async function submitPledge() {
  pledgeError.value = ''
  if (!pledgeAmount.value || pledgeAmount.value < 100) { pledgeError.value = 'Минимальная сумма — 100 ₸'; return }
  pledgeLoading.value = true
  try {
    const { data } = await projectsApi.pledge(project.value.id, {
      rewardId: selectedReward.value?.id || null,
      amount:   pledgeAmount.value
    })
    newBalance.value = data.newBalance
    auth.updateUser({ balance: data.newBalance })
    project.value.collectedAmount += pledgeAmount.value
    project.value.backersCount++
    pledgeStep.value = 1
  } catch (e) {
    pledgeError.value = e.response?.data?.message || 'Ошибка при оплате'
  }
  pledgeLoading.value = false
}

function fmtMoney(n) { return new Intl.NumberFormat('ru-KZ', { maximumFractionDigits: 0 }).format(n || 0) + ' ₸' }
function fmtDate(d)  { return new Date(d).toLocaleDateString('ru-KZ', { day:'numeric', month:'long', year:'numeric' }) }
function nl2br(str)  { return (str || '').replace(/\n/g, '<br/>') }
function embedUrl(url) {
  const m = url.match(/(?:youtube\.com\/watch\?v=|youtu\.be\/)([^&\s]+)/)
  return m ? `https://www.youtube.com/embed/${m[1]}` : url
}
const MONTHS = ['','Январь','Февраль','Март','Апрель','Май','Июнь','Июль','Август','Сентябрь','Октябрь','Ноябрь','Декабрь']
function monthName(n) { return MONTHS[n] || '' }

function share(platform) {
  const url = encodeURIComponent(window.location.href)
  const text = encodeURIComponent(`Поддержи проект: ${project.value.title}`)
  if (platform === 'telegram') window.open(`https://t.me/share/url?url=${url}&text=${text}`)
  if (platform === 'whatsapp') window.open(`https://wa.me/?text=${text}%20${url}`)
}
async function copyLink() {
  await navigator.clipboard.writeText(window.location.href)
  copied.value = true; setTimeout(() => copied.value = false, 2000)
}
</script>

<style scoped>
.back-btn { background: none; border: none; color: var(--text-muted); cursor: pointer; font-size: 14px; font-family: 'DM Sans', sans-serif; margin-bottom: 28px; padding: 0; transition: color .2s; }
.back-btn:hover { color: var(--gold); }

.detail-layout { display: grid; grid-template-columns: 1fr 380px; gap: 48px; align-items: start; }

.detail-cover { width: 100%; height: 400px; border-radius: var(--r-lg); overflow: hidden; margin-bottom: 20px; background: var(--sand); }
.detail-cover img { width: 100%; height: 100%; object-fit: cover; }
.cover-placeholder { width: 100%; height: 100%; display: flex; align-items: center; justify-content: center; font-size: 80px; background: linear-gradient(135deg, var(--gold-subtle), var(--sand)); }

.detail-meta { display: flex; align-items: center; gap: 16px; margin-bottom: 14px; flex-wrap: wrap; }
.meta-city, .meta-views { font-size: 13px; color: var(--text-muted); }
.detail-title { font-size: clamp(1.5rem, 3vw, 2.2rem); margin-bottom: 14px; }
.detail-short { font-size: 16px; color: var(--text-secondary); line-height: 1.7; margin-bottom: 28px; }

.video-wrap { border-radius: var(--r-md); overflow: hidden; margin-bottom: 24px; }
.video-wrap iframe { width: 100%; height: 320px; border: none; }
.detail-body { font-size: 15px; line-height: 1.8; color: var(--text-secondary); }
.media-grid { display: grid; grid-template-columns: repeat(3,1fr); gap: 12px; margin-top: 24px; }
.media-img  { width: 100%; height: 160px; object-fit: cover; border-radius: var(--r-sm); }

.author-tab { padding: 8px 0; }
.author-profile { display: flex; gap: 20px; align-items: flex-start; }
.author-avatar { width: 72px; height: 72px; border-radius: 50%; object-fit: cover; border: 3px solid var(--gold-pale); }
.author-avatar-ph { width: 72px; height: 72px; border-radius: 50%; background: linear-gradient(135deg, var(--gold), var(--gold-light)); display: flex; align-items: center; justify-content: center; font-size: 28px; color: #fff; font-weight: 700; flex-shrink: 0; }
.author-name { font-size: 1.2rem; font-weight: 700; font-family: 'Cormorant Garamond', serif; }

.updates-list { display: flex; flex-direction: column; gap: 20px; }
.update-card { background: var(--gold-subtle); border: 1px solid var(--border); border-radius: var(--r-md); padding: 20px; }
.update-date  { font-size: 12px; color: var(--text-muted); margin-bottom: 8px; }
.update-title { margin-bottom: 8px; }

/* Sidebar */
.support-card { position: sticky; top: 88px; }
.support-amount { font-size: 2.2rem; font-weight: 700; color: var(--gold); }
.support-goal   { font-size: 14px; margin-top: 4px; }
.support-stats  { display: flex; align-items: center; gap: 0; }
.ss-item  { flex: 1; text-align: center; }
.ss-val   { font-family: 'Cormorant Garamond', serif; font-size: 1.6rem; font-weight: 700; color: var(--text-primary); }
.ss-lbl   { font-size: 11px; color: var(--text-muted); margin-top: 2px; }
.ss-sep   { width: 1px; height: 32px; background: var(--border); flex-shrink: 0; }

.rewards-section { margin-top: 24px; padding-top: 20px; border-top: 1px solid var(--border); }
.rewards-title   { font-size: 11px; font-weight: 700; text-transform: uppercase; letter-spacing: .07em; color: var(--text-muted); margin-bottom: 14px; }
.reward-opt      { border: 1.5px solid var(--border); border-radius: var(--r-md); padding: 16px; margin-bottom: 10px; cursor: pointer; transition: all .2s; }
.reward-opt:hover   { border-color: var(--gold); background: var(--gold-subtle); }
.reward-opt.selected { border-color: var(--gold); background: var(--gold-pale); }
.ro-amount  { font-weight: 700; color: var(--gold); font-size: 16px; margin-bottom: 4px; }
.ro-title   { font-weight: 700; font-size: 14px; margin-bottom: 6px; }
.ro-desc    { font-size: 13px; line-height: 1.5; margin-bottom: 8px; }
.ro-delivery, .ro-limit { font-size: 12px; color: var(--text-muted); margin-top: 4px; }

.share-title { font-size: 13px; font-weight: 700; color: var(--text-muted); margin-bottom: 12px; text-transform: uppercase; letter-spacing: .06em; }
.share-btns  { display: flex; gap: 8px; flex-wrap: wrap; }
.share-btn   { padding: 8px 14px; border-radius: var(--r-sm); border: 1px solid var(--border); background: var(--sand); font-size: 13px; font-weight: 600; cursor: pointer; color: var(--text-secondary); transition: all .2s; font-family: 'DM Sans', sans-serif; }
.share-btn:hover { border-color: var(--gold); color: var(--gold); background: var(--gold-subtle); }

.empty-state { text-align: center; padding: 40px 0; }
.empty-icon  { font-size: 40px; margin-bottom: 12px; }

@media (max-width: 960px) { .detail-layout { grid-template-columns: 1fr; } .support-card { position: static; } }
</style>
