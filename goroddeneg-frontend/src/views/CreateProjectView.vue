<template>
  <div class="page">
    <div class="container" style="max-width:860px">
      <h1 class="mb-2">Создание проекта</h1>
      <p class="text-muted mb-6">Заполните все разделы, чтобы запустить проект на платформе</p>

      <!-- Step navigation -->
      <div class="step-nav">
        <div
          v-for="(s, i) in steps"
          :key="s"
          class="step-item"
          :class="{ active: step === i, done: step > i }"
          @click="step > i && (step = i)"
        >
          <div class="step-num">{{ step > i ? '✓' : i + 1 }}</div>
          <span>{{ s }}</span>
        </div>
      </div>

      <!-- STEP 0: Основное -->
      <div v-if="step === 0">
        <div class="form-group">
          <label class="form-label">Название проекта *</label>
          <input v-model="form.title" class="form-control" placeholder="Краткое и запоминающееся название" maxlength="255" />
          <div class="form-hint">{{ form.title.length }}/255 символов</div>
        </div>

        <div class="form-group">
          <label class="form-label">Короткое описание * (для превью)</label>
          <textarea v-model="form.shortDesc" class="form-control" placeholder="1–2 предложения, которые привлекут внимание спонсоров" maxlength="500" style="min-height:80px" />
          <div class="form-hint">{{ form.shortDesc.length }}/500 символов</div>
        </div>

        <div class="form-group">
          <label class="form-label">Обложка проекта</label>
          <div class="upload-box" @click="$refs.coverInput.click()">
            <img v-if="coverPreview" :src="coverPreview" style="max-height:200px;border-radius:8px" />
            <template v-else>
              <div class="upload-box-icon">🖼️</div>
              <div class="upload-box-text"><strong>Нажмите для загрузки</strong><br/>PNG, JPG, WebP — до 5 МБ</div>
            </template>
          </div>
          <input ref="coverInput" type="file" accept="image/*" style="display:none" @change="onCoverChange" />
        </div>

        <div class="form-group">
          <label class="form-label">Подробное описание</label>
          <textarea v-model="form.fullDesc" class="form-control" placeholder="Расскажите всё о вашем проекте — историю, цели, команду, план реализации..." style="min-height:180px" />
        </div>

        <div class="form-group">
          <label class="form-label">Ссылка на видео (YouTube)</label>
          <input v-model="form.videoUrl" class="form-control" placeholder="https://youtube.com/watch?v=..." />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Категория *</label>
            <select v-model="form.categoryId" class="form-control">
              <option value="">Выберите категорию</option>
              <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.icon }} {{ c.name }}</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">Город</label>
            <select v-model="form.city" class="form-control">
              <option value="">Выберите город</option>
              <option v-for="c in cities" :key="c" :value="c">{{ c }}</option>
            </select>
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Необходимая сумма (₸) *</label>
            <input v-model.number="form.goalAmount" type="number" class="form-control" placeholder="1 000 000" min="1000" />
          </div>
          <div class="form-group">
            <label class="form-label">Срок сбора (дней, макс. 90) *</label>
            <input v-model.number="form.durationDays" type="number" class="form-control" placeholder="30" min="1" max="90" />
          </div>
        </div>

        <div class="hidden-toggle">
          <input type="checkbox" v-model="form.isHidden" id="hidden" />
          <label for="hidden">
            <strong>Скрытый проект</strong> — доступен только по прямой ссылке, не отображается в каталоге
          </label>
        </div>
      </div>

      <!-- STEP 1: Вознаграждения -->
      <div v-if="step === 1">
        <div class="alert alert-gold mb-6">
          💡 Вознаграждения мотивируют спонсоров поддержать ваш проект. Чем интереснее — тем лучше!
        </div>

        <div v-for="(r, i) in form.rewards" :key="i" class="reward-block card card-body mb-4">
          <div class="reward-block-header">
            <h3 class="font-display">Вознаграждение #{{ i + 1 }}</h3>
            <button v-if="form.rewards.length > 0" class="btn btn-sm" style="background:var(--danger-bg);color:var(--danger)" @click="form.rewards.splice(i,1)">✕ Удалить</button>
          </div>
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">Название *</label>
              <input v-model="r.title" class="form-control" placeholder="Ранний сторонник" />
            </div>
            <div class="form-group">
              <label class="form-label">Минимальная сумма (₸) *</label>
              <input v-model.number="r.minAmount" type="number" class="form-control" placeholder="5 000" min="100" />
            </div>
          </div>
          <div class="form-group">
            <label class="form-label">Описание</label>
            <textarea v-model="r.description" class="form-control" placeholder="Что получит спонсор?" style="min-height:80px" />
          </div>
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">Месяц доставки</label>
              <select v-model.number="r.deliveryMonth" class="form-control">
                <option value="">Не указан</option>
                <option v-for="(m,mi) in months" :key="mi" :value="mi+1">{{ m }}</option>
              </select>
            </div>
            <div class="form-group">
              <label class="form-label">Год доставки</label>
              <select v-model.number="r.deliveryYear" class="form-control">
                <option value="">Не указан</option>
                <option v-for="y in [2025,2026,2027,2028]" :key="y" :value="y">{{ y }}</option>
              </select>
            </div>
          </div>
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">Способ доставки</label>
              <select v-model="r.shippingType" class="form-control">
                <option>Не требует доставки</option>
                <option>За счёт автора</option>
                <option>За счёт спонсора</option>
              </select>
            </div>
            <div class="form-group">
              <label class="form-label">Лимит (оставьте пустым = без лимита)</label>
              <input v-model.number="r.limitCount" type="number" class="form-control" placeholder="∞" min="1" />
            </div>
          </div>
        </div>

        <button class="btn btn-outline btn-full" @click="addReward">+ Добавить вознаграждение</button>
      </div>

      <!-- STEP 2: О себе -->
      <div v-if="step === 2">
        <div class="alert alert-info mb-6">
          ℹ️ Информация о вас берётся из вашего профиля. Вы можете обновить её в <router-link to="/cabinet">личном кабинете</router-link>.
        </div>
        <div class="profile-preview card card-body">
          <div class="pp-row">
            <div class="pp-avatar">{{ auth.user?.firstName?.[0] }}{{ auth.user?.lastName?.[0] }}</div>
            <div>
              <div class="pp-name">{{ auth.user?.firstName }} {{ auth.user?.lastName }}</div>
              <div class="pp-email text-muted">{{ auth.user?.email }}</div>
              <div class="pp-city text-muted" v-if="auth.user?.city">📍 {{ auth.user?.city }}</div>
            </div>
          </div>
          <p class="pp-bio text-muted mt-4" v-if="auth.user?.bio">{{ auth.user?.bio }}</p>
        </div>
      </div>

      <!-- STEP 3: Счета (Legal) -->
      <div v-if="step === 3">
        <div class="form-group">
          <label class="form-label">Страна</label>
          <select class="form-control" disabled>
            <option>Казахстан</option>
          </select>
          <div class="form-hint">На данный момент платформа работает только в Казахстане</div>
        </div>
        <div class="form-group">
          <label class="form-label">Кто вы? *</label>
          <div class="legal-type-btns">
            <button v-for="t in legalTypes" :key="t.val" class="legal-type-btn" :class="{ active: legal.legalType === t.val }" @click="legal.legalType = t.val">
              {{ t.icon }} {{ t.label }}
            </button>
          </div>
        </div>
        <template v-if="legal.legalType">
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">Банк</label>
              <input v-model="legal.bankName" class="form-control" placeholder="Kaspi Bank" />
            </div>
            <div class="form-group">
              <label class="form-label">БИК банка</label>
              <input v-model="legal.bik" class="form-control" placeholder="CASPKZKA" />
            </div>
          </div>
          <div class="form-group">
            <label class="form-label">Расчётный счёт</label>
            <input v-model="legal.accountNumber" class="form-control" placeholder="KZ00 0000 0000 0000 0000" />
          </div>
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">ФИО</label>
              <input v-model="legal.fullName" class="form-control" placeholder="Иванов Иван Иванович" />
            </div>
            <div class="form-group">
              <label class="form-label">ИИН</label>
              <input v-model="legal.iin" class="form-control" placeholder="000000000000" maxlength="12" />
            </div>
          </div>
          <div class="form-group">
            <label class="form-label">Адрес регистрации</label>
            <input v-model="legal.regAddress" class="form-control" placeholder="г. Алматы, ул. Абая, д. 10" />
          </div>
          <template v-if="legal.legalType === 'Organization'">
            <hr class="divider" />
            <div class="form-group">
              <label class="form-label">Полное наименование организации</label>
              <input v-model="legal.orgName" class="form-control" />
            </div>
            <div class="form-row">
              <div class="form-group"><label class="form-label">БИН компании</label><input v-model="legal.bin" class="form-control" maxlength="12" /></div>
              <div class="form-group"><label class="form-label">Генеральный директор</label><input v-model="legal.orgDirector" class="form-control" /></div>
            </div>
          </template>
        </template>
      </div>

      <!-- STEP 4: Проверка и пакет -->
      <div v-if="step === 4">
        <!-- Summary -->
        <div class="summary-card card card-body mb-6">
          <h3 class="font-display mb-4">Сводка по проекту</h3>
          <div class="summary-row"><span class="sr-label">Название</span><span>{{ form.title }}</span></div>
          <div class="summary-row"><span class="sr-label">Цель</span><span class="text-gold">{{ fmtMoney(form.goalAmount) }}</span></div>
          <div class="summary-row"><span class="sr-label">Срок</span><span>{{ form.durationDays }} дней</span></div>
          <div class="summary-row"><span class="sr-label">Вознаграждений</span><span>{{ form.rewards.length }}</span></div>
        </div>

        <!-- Packages -->
        <h3 class="font-display mb-4">Выберите пакет размещения</h3>
        <div class="packages-grid">
          <div v-for="pkg in packages" :key="pkg.id" class="pkg-card" :class="{ selected: selectedPkg === pkg.id }" @click="selectedPkg = pkg.id">
            <div class="pkg-name">{{ pkg.name }}</div>
            <div class="pkg-price font-display">{{ pkg.price }}</div>
            <div class="pkg-features">
              <div v-for="f in pkg.features" :key="f.label" class="pkg-feat">
                <span :class="f.has ? 'feat-yes' : 'feat-no'">{{ f.has ? '✓' : '×' }}</span>
                <span>{{ f.label }}</span>
                <span v-if="f.val" class="feat-val">{{ f.val }}</span>
              </div>
            </div>
          </div>
        </div>

        <div v-if="submitError" class="alert alert-danger mt-4">{{ submitError }}</div>
        <div v-if="!selectedPkg" class="alert alert-gold mt-4">⬆️ Выберите пакет для отправки на модерацию</div>
      </div>

      <!-- Navigation buttons -->
      <div class="step-actions">
        <button v-if="step > 0" class="btn btn-ghost" @click="step--">← Назад</button>
        <div style="flex:1" />
        <button v-if="step < steps.length - 1" class="btn btn-primary" @click="nextStep" :disabled="!canNext">Далее →</button>
        <button v-if="step === steps.length - 1" class="btn btn-primary" :disabled="!selectedPkg || submitting" @click="submitProject">
          <span v-if="submitting" class="spinner" style="width:18px;height:18px" />
          <span v-else>🚀 Отправить на модерацию</span>
        </button>
      </div>
    </div>

    <!-- Success modal -->
    <transition name="fade">
      <div v-if="successModal" class="modal-overlay">
        <div class="modal-box">
          <div class="modal-body" style="text-align:center;padding:48px 40px">
            <div style="font-size:64px;margin-bottom:20px">🎉</div>
            <h2 class="font-display mb-4">Проект отправлен!</h2>
            <p class="text-muted mb-6">Ваш проект отправлен на модерацию. Мы рассмотрим его в течение 1–3 рабочих дней и уведомим вас по email.</p>
            <router-link to="/cabinet" class="btn btn-primary">Перейти в кабинет →</router-link>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { projectsApi, categoriesApi, usersApi } from '@/services/api'

const auth       = useAuthStore()
const step       = ref(0)
const categories = ref([])
const selectedPkg = ref(null)
const submitting  = ref(false)
const submitError = ref('')
const successModal = ref(false)
const coverPreview = ref('')
const coverFile    = ref(null)
const createdId    = ref(null)

const steps = ['Основное','Вознаграждение','О себе','Счета','Проверка']
const cities = ['Алматы','Нур-Султан','Шымкент','Актобе','Тараз','Павлодар','Усть-Каменогорск','Семей','Атырау','Костанай','Другой']
const months = ['Январь','Февраль','Март','Апрель','Май','Июнь','Июль','Август','Сентябрь','Октябрь','Ноябрь','Декабрь']
const legalTypes = [
  { val:'Individual',   icon:'👤', label:'Физическое лицо' },
  { val:'IE',           icon:'🏢', label:'ИП' },
  { val:'Organization', icon:'🏛️', label:'Организация' },
]
const packages = [
  { id:'Start',    name:'Старт',      price:'25 000 ₸', features:[{label:'Размещение проекта',has:true},{label:'Консультация',has:true},{label:'Рассылка подписчикам',has:false},{label:'Срок размещения',has:false,val:'45 дней'},{label:'Приоритетное размещение',has:false}] },
  { id:'Advanced', name:'Продвинутый', price:'39 000 ₸', features:[{label:'Размещение проекта',has:true},{label:'Консультация',has:true},{label:'Рассылка подписчикам',has:true},{label:'Срок размещения',has:false,val:'60 дней'},{label:'Приоритетное размещение',has:true}] },
]

const form = reactive({
  title:'', shortDesc:'', fullDesc:'', videoUrl:'',
  categoryId:'', city:'', goalAmount:null, durationDays:30,
  isHidden:false, rewards:[]
})
const legal = reactive({ legalType:'', bankName:'', bik:'', accountNumber:'', fullName:'', iin:'', bin:'', regAddress:'', orgName:'', orgDirector:'' })

const canNext = computed(() => {
  if (step.value === 0) return form.title && form.shortDesc && form.categoryId && form.goalAmount > 0 && form.durationDays > 0
  return true
})

function addReward() {
  form.rewards.push({ title:'', minAmount:null, description:'', deliveryMonth:null, deliveryYear:null, shippingType:'Не требует доставки', limitCount:null })
}

function onCoverChange(e) {
  const file = e.target.files[0]
  if (!file) return
  coverFile.value = file
  coverPreview.value = URL.createObjectURL(file)
}

function nextStep() { if (canNext.value) step.value++ }

async function submitProject() {
  submitting.value = true; submitError.value = ''
  try {
    // 1. Create project
    const { data: created } = await projectsApi.create({
      title: form.title, shortDesc: form.shortDesc, fullDesc: form.fullDesc,
      videoUrl: form.videoUrl, categoryId: Number(form.categoryId),
      city: form.city, goalAmount: form.goalAmount, durationDays: form.durationDays,
      isHidden: form.isHidden,
      rewards: form.rewards.filter(r => r.title && r.minAmount)
    })
    createdId.value = created.id

    // 2. Upload cover if selected
    if (coverFile.value) {
      try { await projectsApi.uploadCover(created.id, coverFile.value) } catch {}
    }

    // 3. Save legal
    if (legal.legalType) {
      try { await usersApi.saveLegal({ ...legal }) } catch {}
    }

    // 4. Select package → sends to moderation
    await projectsApi.selectPkg(created.id, { packageType: selectedPkg.value, paymentRef: `PKG-${Date.now()}` })

    successModal.value = true
  } catch (e) {
    submitError.value = e.response?.data?.message || 'Ошибка при создании проекта'
  }
  submitting.value = false
}

function fmtMoney(n) { return new Intl.NumberFormat('ru-KZ', { maximumFractionDigits: 0 }).format(n || 0) + ' ₸' }

onMounted(async () => {
  const { data } = await categoriesApi.getAll()
  categories.value = data
})
</script>

<style scoped>
.hidden-toggle { display: flex; align-items: flex-start; gap: 10px; background: var(--gold-subtle); border: 1px solid var(--border); border-radius: var(--r-sm); padding: 14px 16px; margin-top: 8px; font-size: 13px; }
.hidden-toggle input { margin-top: 2px; accent-color: var(--gold); }

.reward-block-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }

.profile-preview { }
.pp-row   { display: flex; gap: 16px; align-items: center; }
.pp-avatar { width: 56px; height: 56px; border-radius: 50%; background: linear-gradient(135deg, var(--gold), var(--gold-light)); color: #fff; font-size: 20px; font-weight: 700; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
.pp-name  { font-weight: 700; font-size: 16px; }
.pp-email, .pp-city { font-size: 13px; margin-top: 3px; }

.legal-type-btns { display: flex; gap: 10px; flex-wrap: wrap; }
.legal-type-btn  { padding: 10px 20px; border-radius: var(--r-md); border: 1.5px solid var(--border); background: var(--warm-white); font-size: 14px; font-weight: 600; cursor: pointer; color: var(--text-muted); transition: all .2s; font-family: 'DM Sans', sans-serif; }
.legal-type-btn.active { background: var(--gold-pale); border-color: var(--gold); color: var(--gold); }

.summary-card { border: 1px solid var(--border); }
.summary-row  { display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid var(--border-light); font-size: 14px; }
.summary-row:last-child { border: none; }
.sr-label { color: var(--text-muted); }

.packages-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
.pkg-card { border: 2px solid var(--border); border-radius: var(--r-md); padding: 28px; cursor: pointer; transition: all .2s; }
.pkg-card:hover   { border-color: var(--gold-light); }
.pkg-card.selected { border-color: var(--gold); background: var(--gold-subtle); box-shadow: var(--shadow-gold); }
.pkg-name  { font-size: 18px; font-weight: 700; margin-bottom: 8px; }
.pkg-price { font-size: 2rem; color: var(--gold); margin-bottom: 20px; }
.pkg-feat  { display: flex; align-items: center; gap: 8px; font-size: 13px; color: var(--text-muted); margin-bottom: 8px; }
.feat-yes  { color: var(--success); font-weight: 800; }
.feat-no   { color: var(--border); font-weight: 800; }
.feat-val  { margin-left: auto; font-weight: 600; color: var(--text-primary); }

.step-actions { display: flex; align-items: center; margin-top: 40px; padding-top: 24px; border-top: 1px solid var(--border); gap: 12px; }

@media (max-width: 600px) { .packages-grid { grid-template-columns: 1fr; } }
</style>
