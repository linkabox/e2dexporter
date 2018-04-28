
local json = require "json"
local inspect = require "inspect"

function print_table(t)
  print(inspect(t))
end

function readAll(file)
    local f = assert(io.open(file, "rb"))
    local content = f:read("*all")
    f:close()
    return content
end

function writeAll(path,data)
  local f = assert(io.open(path,"w"))
  f:write(data)
  f:close()
end

local coco_file = "characters2"
--local coco_file = "test"
local data = readAll(coco_file..".coco.bytes")
local texHeight = 2048

--print(data)

local env = {}
local meta = {
  name = coco_file,
  texCount = 0,
  sprites = {},
  animations = {}
}

local anim_ids = {}

local function to_rect(src)
  local x1,x2,x3,x4 = src[1],src[3],src[5],src[7]
    local y1,y2,y3,y4 = src[2],src[4],src[6],src[8]
    local rect = {
      x = math.min(x1,x2,x3,x4),
      y = texHeight - math.max(y1,y2,y3,y4),  --ejoy2d的y轴与unity相反
      width = math.max(x1,x2,x3,x4) - math.min(x1,x2,x3,x4),
      height = math.max(y1,y2,y3,y4) - math.min(y1,y2,y3,y4)
    }
    return rect
end

local types = {"texture","picture","animation"}
for _,v in ipairs(types) do
  rawset(env,v,function(arg)
    if v == "texture" then
      meta.texCount = arg
    elseif v == "picture" then
      local sprite = {}
      sprite.id = arg.id
      sprite.name = "pic_"..arg.id
      sprite.texId = arg[1].tex
      sprite.src = to_rect(arg[1].src)
      sprite.screen = to_rect(arg[1].screen)
      table.insert(meta.sprites,sprite)
    elseif v == "animation" then
      anim_ids[arg.id] = true
      local animation = {}
      animation.id = arg.id
      animation.name = arg.export or "unknow_anim_"..arg.id
      animation.components = {}
      for _,v in ipairs(arg.component) do
        table.insert(animation.components,v.id)
      end
      animation.frames = {}
      local frames = arg[1]
      local max_render_count = 0
      for _,v in ipairs(frames) do
        local t_count = #v
        max_render_count = math.max(t_count,max_render_count)
        table.insert(animation.frames,v)
      end
      animation.max_render_count = max_render_count
      --检查一下哪些不是单位矩阵
--      for _,frame in ipairs(animation.frames) do
--        for _,trans in ipairs(frame) do
--          local mat = trans.mat
--          if mat[1] ~= 1024 or mat[2] ~= 0 or mat[3] ~= 0 or mat[4] ~= 1024 then
--            print("find anim_id:"..arg.id)
--            print_table(mat)
--          end
--        end
--      end
      table.insert(meta.animations,animation)
    end
  end)
end

load(data,"coco","t",env)()

--print_table(meta)
--print(meta.texture)
--print(#meta.sprites)
--print(#meta.animations)

--local json_data = json:encode_pretty(meta)
--print(json_data)
--writeAll(coco_file.."_json.json",json_data)


local json_data = json:encode_pretty(meta)
--print(json_data)
writeAll(coco_file.."_pak.json",json_data)
print("export animations:",#meta.sprites)
print("export animations:",#meta.animations)


